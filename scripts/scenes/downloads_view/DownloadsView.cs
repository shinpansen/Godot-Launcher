using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Binding.Controls;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
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

    private Button _buttonX86 => GetNode<Button>("%ButtonX86");
    private Button _buttonX64 => GetNode<Button>("%ButtonX64");
    private OptionButton _optionsVersions => GetNode< OptionButton>("%OptionsVersions");
    private OptionButton _optionsTypes => GetNode<OptionButton>("%OptionsTypes");
    private CheckButton _checkButtonMono => GetNode<CheckButton>("%CheckButtonMono");

    private DownloadConfig _config = new();

    public void Load()
    {
        _buttonX86.ButtonPressed = !System.Environment.Is64BitOperatingSystem;
        _buttonX64.ButtonPressed = System.Environment.Is64BitOperatingSystem;

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
        //Download urls
        string version = GetSelectedVersion();
        string type = GetSelectedType();
        List<DownloadUrl> urls;
        try
        {
            urls = GodotDownloader.GetGodotArchiveDownloadUrls(version, type);
        }
        catch (Exception ex)
        {
            ErrorTools.ShowError($"Can't fetch download links for version {version}-{type} : {ex.Message}");
            return;
        }

        //Match url
        bool mono = _checkButtonMono.ButtonPressed;
        bool version32bits = _buttonX86.ButtonPressed;
        string finalUrl = GodotDownloader.FilterUrls(urls, version, type, mono, version32bits);
        if (string.IsNullOrEmpty(finalUrl))
        {
            ErrorTools.ShowError($"Can't find download url for those parameters");
            return;
        }

        //Download
        string outputPath;
        try
        {
            outputPath = GodotDownloader.DownloadGodotZip(finalUrl, version, type, mono, version32bits);
        }
        catch (Exception ex)
        {
            ErrorTools.ShowError($"Error downloading Godot_v{version}-{type} archive: {ex.Message}");
            return;
        }

        //Extract
        try
        {
            GodotDownloader.ExtractZip(outputPath);
        }
        catch (Exception ex)
        {
            ErrorTools.ShowError($"Error extracting archive at {outputPath}: {ex.Message}");
            return;
        }

        //Scan
        try
        {
            string errors = string.Empty;
            VersionsConfig versionsConfig = VersionsView?.Refresh(out errors);
            ProjectsView?.RefreshProjectsVersions(versionsConfig.Versions);
            
            if(!string.IsNullOrEmpty(errors))
                ErrorTools.ShowError(errors);
        }
        catch (Exception ex)
        {
            ErrorTools.ShowError(ex.Message);
        }

        ErrorTools.ShowError("OK !!!!");
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
}
