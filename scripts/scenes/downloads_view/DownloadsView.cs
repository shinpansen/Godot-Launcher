using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Binding.Controls;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Reflection.Metadata;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Scenes.DownloadsView;

public partial class DownloadsView : Node
{
    [Export]
    public VersionsView.VersionsView VersionsView { get; set; }

    [Export]
    public ProjectsView.ProjectsView ProjectsView { get; set; }

    private Button _buttonDownload => GetNode<Button>("%ButtonDownload");
    private Button _buttonX86 => GetNode<Button>("%ButtonX86");
    private Button _buttonX64 => GetNode<Button>("%ButtonX64");
    private OptionButton _optionsVersions => GetNode< OptionButton>("%OptionsVersions");
    private OptionButton _optionsTypes => GetNode<OptionButton>("%OptionsTypes");
    private CheckButton _checkButtonMono => GetNode<CheckButton>("%CheckButtonMono");
    private HBoxContainer _loaderHBoxContainer => GetNode<HBoxContainer>("%LoaderHBoxContainer");
    private Label _labelProgress => GetNode<Label>("%LabelProgress");

    private Theme _primaryButtonTheme = GD.Load<Theme>("res://assets/themes/button-primary-theme.tres");
    private Theme _secondaryButtonTheme = GD.Load<Theme>("res://assets/themes/button-secondary-theme.tres");

    private DownloadConfig _config = new();
    private DownloadRequest _request;

    private BackgroundWorker _downloadWorker;

    public override void _Ready()
    {
        _buttonX86.ButtonPressed = !System.Environment.Is64BitOperatingSystem;
        _buttonX64.ButtonPressed = System.Environment.Is64BitOperatingSystem;

        _downloadWorker = new BackgroundWorker
        {
            WorkerSupportsCancellation = true,
            WorkerReportsProgress = true,
        };
        _downloadWorker.DoWork += DownloadWorkerDoWork;
        _downloadWorker.RunWorkerCompleted += DownloadWorkerOnCompleted;
    }

    public override void _ExitTree()
    {
        SystemTools.CancelWorker(_downloadWorker);
    }

    public void Load()
    {
        _loaderHBoxContainer.Visible = _downloadWorker.IsBusy;
        _config.AvailableReleases = GodotDownloader.DownloadReleasesInfo();

        int i = 0;
        _optionsVersions.Clear();
        _config.AvailableReleases.ForEach(v =>
        {
            _optionsVersions.GetPopup().AddItem(v.Name, i++);
        });

        _optionsVersions.Selected = 0;
        OnOptionsVersionsItemSelected(0);
    }

    private void OnOptionsVersionsItemSelected(long index)
    {
        Release selectedRelease = _config.AvailableReleases[(int)index];
        _config.AvailableTypes = selectedRelease.Releases.Select(r =>  r.Name).ToList();

        int i = 0;
        _optionsTypes.Clear();
        _config.AvailableTypes.ForEach(r =>
        {
            _optionsTypes.GetPopup().AddItem(r, i++);
        });
        _optionsTypes.Selected = 0;

        if (new Version(selectedRelease.Name).Major < 3)
        {
            _config.MonoAvailable = false;
            _checkButtonMono.ButtonPressed = false;
        }
        else
            _config.MonoAvailable = true;
        _checkButtonMono.Visible = _config.MonoAvailable;
    }

    private void OnButtonDownloadDown()
    {
        if (_downloadWorker.IsBusy)
        {
            ChangeProgressText(TranslationServer.Translate("!canceldownload"));
            _downloadWorker.CancelAsync();
        }
        else
        {
            _buttonDownload.Theme = _secondaryButtonTheme;
            _buttonDownload.Text = TranslationServer.Translate("!cancel");
            _loaderHBoxContainer.Visible = true;
            _labelProgress.Text = string.Empty;
            _request = new DownloadRequest(GetSelectedVersion(), GetSelectedType(), GetMono(), Get32Bits());
            _downloadWorker.RunWorkerAsync();
        }
    }

    private string GetSelectedVersion()
    {
        int index = _optionsVersions.Selected;
        Release selectedRelease = _config.AvailableReleases[index];
        return selectedRelease.Name;
    }

    private string GetSelectedType()
    {
        int index = _optionsTypes.Selected;
        return _config.AvailableTypes[index];
    }

    private bool GetMono() => _checkButtonMono.ButtonPressed;
    private bool Get32Bits() => _buttonX86.ButtonPressed;

    private void ChangeProgressText(string text)
    {
        _labelProgress.Text = text;
    }

    private void DownloadWorkerDoWork(object sender, DoWorkEventArgs e)
    {
        //Download urls
        CallDeferred(nameof(ChangeProgressText), TranslationServer.Translate("!fetching"));
        string version = _request.Version;
        string type = _request.Type;
        List<DownloadUrl> urls;
        try
        {
            urls = GodotDownloader.GetGodotArchiveDownloadUrls(version, type);
        }
        catch (Exception ex)
        {
            e.Result = new DownloadResult(false, TranslationServer.Translate("!cantfetch") + $" {version}-{type} : {ex.Message}");
            return;
        }
        if (CheckForCancellation(e)) return;

        //Match url
        bool mono = _request.Moono;
        bool version32bits = _request.Version32bits;
        string finalUrl = GodotDownloader.FilterUrls(urls, version, type, mono, version32bits);
        if (string.IsNullOrEmpty(finalUrl))
        {
            e.Result = new DownloadResult(false, TranslationServer.Translate("!nourl"));
            return;
        }
        if (CheckForCancellation(e)) return;

        //Download
        CallDeferred(nameof(ChangeProgressText), TranslationServer.Translate("!downloading"));
        string outputPath;
        try
        {
            outputPath = GodotDownloader.DownloadGodotZip(finalUrl, version, type, mono, version32bits);
        }
        catch (Exception ex)
        {
            string errMessage = TranslationServer.Translate("!errdownload");
            e.Result = new DownloadResult(false, string.Format(errMessage, version, type, ex.Message));
            return;
        }
        if (CheckForCancellation(e))
        {
            try
            {
                System.IO.File.Delete(outputPath);
            }
            catch (Exception)
            {
            }
            return;
        }

        //Extract
        string outputDirectory = System.IO.Path.GetDirectoryName(outputPath);
        CallDeferred(nameof(ChangeProgressText), TranslationServer.Translate("!extracting"));
        try
        {
            GodotDownloader.ExtractZip(outputPath);
        }
        catch (Exception ex)
        {
            string errMessage = TranslationServer.Translate("!errextracting");
            e.Result = new DownloadResult(false, string.Format(errMessage, outputPath, ex.Message));
            return;
        }
        if (CheckForCancellation(e))
        {
            try
            {
                System.IO.Directory.Delete(outputDirectory, true);
            }
            catch (Exception)
            {
            }
            return;
        }

        //Scan
        CallDeferred(nameof(ChangeProgressText), TranslationServer.Translate("!scanning"));
        try
        {
            string errors = string.Empty;
            VersionsConfig versionsConfig = VersionsView?.Refresh(out errors);
            ProjectsView?.RefreshProjectsVersions(versionsConfig.Versions);

            if (!string.IsNullOrEmpty(errors))
                e.Result = new DownloadResult(false, errors);
        }
        catch (Exception ex)
        {
            e.Result = new DownloadResult(false, ex.Message);
        }
        e.Result = new DownloadResult(true);
    }

    private bool CheckForCancellation(DoWorkEventArgs e)
    {
        if(_downloadWorker.CancellationPending)
        {
            e.Result = new DownloadResult(false, TranslationServer.Translate("!dlcancelled"));
            return true;
        }
        return false;
    }

    private void DownloadWorkerOnCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        _loaderHBoxContainer.Visible = false;
        _buttonDownload.Theme = _primaryButtonTheme;
        _buttonDownload.Text = TranslationServer.Translate("!download");
        if (e.Result is DownloadResult result)
        {
            if (result.Success)
            {
                DialogTools.ShowMessage(
                    TranslationServer.Translate("!success"),
                    TranslationServer.Translate("!dlcomplete"));
            }
            else
                DialogTools.ShowError(result.Errors);
        }
    }
}
