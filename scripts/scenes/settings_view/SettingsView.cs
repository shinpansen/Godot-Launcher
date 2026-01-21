using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Enums;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes.VersionsView;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GodotLauncher.Scripts.Scenes.SettingsView;

public partial class SettingsView : DataSourceBinding<Settings>
{
    [Export]
    public VersionsView.VersionsView VersionsView { get; set; }

    [Export]
    public ProjectsView.ProjectsView ProjectsView { get; set; }

    protected override Settings LoadDataSource() => UserDataLoader.LoadUserSettings();

    private ScrollContainer _scrollContainer => GetNode<ScrollContainer>("%ScrollContainer");
    private FileDialog _fileDialogCustomPaths => GetNode<FileDialog>("%FileDialogCustomPaths");
    private FileDialog _fileDialogProjects => GetNode<FileDialog>("%FileDialogProjects");
    private FileDialog _fileDialogExcludedFiles => GetNode<FileDialog>("%FileDialogExcludedFiles");
    private Button _buttonScanEngines => GetNode<Button>("%ButtonScanEngines");
    private TextureRect _loaderScanEngines => GetNode<TextureRect>("%LoaderScanEngines");
    private Label _labelScanEngineResult => GetNode<Label>("%LabelScanEnginesResult");
    private Button _buttonScanProjects => GetNode<Button>("%ButtonScanProjects");
    private TextureRect _loaderScanProjects => GetNode<TextureRect>("%LoaderScanProjects");
    private Label _labelScanProjectsResult => GetNode<Label>("%LabelScanProjectsResult");


    private BackgroundWorker _scanEnginesWorker;
    private BackgroundWorker _scanProjectsWorker;

    public override void _Ready()
    {
        _scrollContainer.ClipContents = true;
        OptionButtonLanguagesItemSelected((long)BindingContext.Language);

        _scanEnginesWorker = new BackgroundWorker
        {
            WorkerSupportsCancellation = true
        };
        _scanEnginesWorker.DoWork += ScanEnginesWorkerDoWork;
        _scanEnginesWorker.RunWorkerCompleted += ScanEnginesWorkerOnCompleted;

        _scanProjectsWorker = new BackgroundWorker
        {
            WorkerSupportsCancellation = true
        };
        _scanProjectsWorker.DoWork += ScanProjectsWorkerDoWork;
        _scanProjectsWorker.RunWorkerCompleted += ScanProjectsWorkerOnCompleted;
    }

    public void AddExcludedFile(string path)
    {
        OnExcludedFilesFileSelected(new Godot.Collections.Array<string>() { path });
    }

    private void OptionButtonLanguagesItemSelected(long id)
    {
        Language language = (Language)id;
        LanguageTools.ChangeAppLanguage(language);
        _labelScanEngineResult.Text = string.Empty;
        _labelScanProjectsResult.Text = string.Empty;
    }

    private void OnButtonScanEnginesDown()
    {
        if (_scanEnginesWorker.IsBusy) return;

        StartScanUdateUI(_loaderScanEngines);
        _scanEnginesWorker.RunWorkerAsync();
    }

    private void OnButtonScanProjectsDown()
    {
        if (_scanProjectsWorker.IsBusy) return;

        StartScanUdateUI(_loaderScanProjects);
        _scanProjectsWorker.RunWorkerAsync();
    }

    private void OpenDirSelectionForCustomPaths()
    {
        _fileDialogCustomPaths.Show();
    }

    private void OpenDirSelectionForProjects()
    {
        _fileDialogProjects.Show();
    }

    private void OpenDirSelectionForExcludedFiles()
    {
        _fileDialogExcludedFiles.Show();
    }

    private void OnInstallDirSelected(string dirPath)
    {
        BindingContext.CustomInstallsDirectories.Add(new FileSystemPath(dirPath));
        SetPropertyValue(s => s.CustomInstallsDirectories, BindingContext.CustomInstallsDirectories);
    }

    private void OnExcludedFilesFileSelected(Godot.Collections.Array<string> paths)
    {
        foreach (string filePath in paths)
        {
            BindingContext.ExcludedFiles.Add(new FileSystemPath(filePath));
            SetPropertyValue(s => s.ExcludedFiles, BindingContext.ExcludedFiles);
        }
    }

    private void OnProjectDirSelected(string dirPath)
    {
        BindingContext.ProjectsDirectories.Add(new FileSystemPath(dirPath));
        SetPropertyValue(s => s.ProjectsDirectories, BindingContext.ProjectsDirectories);
    }

    private void DeleteInstallDirectory(Node node)
    {
        if (node is not ItemBinding<FileSystemPath> item) return;

        if (BindingContext.CustomInstallsDirectories.Contains(item.BindingContext))
        {
            var paths = BindingContext.CustomInstallsDirectories;
            paths.Remove(item.BindingContext);
            SetPropertyValue(s => s.CustomInstallsDirectories, paths);
        }
    }

    private void DeleteExcludedFile(Node node)
    {
        if (node is not ItemBinding<FileSystemPath> item) return;

        if (BindingContext.ExcludedFiles.Contains(item.BindingContext))
        {
            var paths = BindingContext.ExcludedFiles;
            paths.Remove(item.BindingContext);
            SetPropertyValue(s => s.ExcludedFiles, paths);
        }
    }

    private void DeleteProjectDirectory(Node node)
    {
        if (node is not ItemBinding<FileSystemPath> item) return;

        if (BindingContext.ProjectsDirectories.Contains(item.BindingContext))
        {
            var paths = BindingContext.ProjectsDirectories;
            paths.Remove(item.BindingContext);
            SetPropertyValue(s => s.ProjectsDirectories, paths);
        }
    }

    private void StartScanUdateUI(TextureRect loader)
    {
        loader.Visible = true;
        _buttonScanEngines.Disabled = true;
        _buttonScanProjects.Disabled = true;
        _labelScanEngineResult.Text = string.Empty;
        _labelScanProjectsResult.Text = string.Empty;
        _buttonScanEngines.MouseDefaultCursorShape = CursorShape.Forbidden;
        _buttonScanProjects.MouseDefaultCursorShape = CursorShape.Forbidden;
    }

    private void EndScanUpdateUI()
    {
        _loaderScanEngines.Visible = false;
        _loaderScanProjects.Visible = false;
        _buttonScanEngines.Disabled = false;
        _buttonScanProjects.Disabled = false;
        _buttonScanEngines.MouseDefaultCursorShape = CursorShape.PointingHand;
        _buttonScanProjects.MouseDefaultCursorShape = CursorShape.PointingHand;
    }

    private void ScanEnginesWorkerDoWork(object sender, DoWorkEventArgs e)
    {
        string errors = string.Empty;
        VersionsConfig versionsConfig = VersionsView?.Refresh(out errors);
        int count = versionsConfig.Versions.Count;
        ProjectsView?.RefreshProjectsVersions(versionsConfig.Versions);
        e.Result = new ScanResult(count, errors);
    }

    private void ScanEnginesWorkerOnCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Result is ScanResult result) 
        {
            if (!string.IsNullOrEmpty(result.Errors))
                ErrorTools.ShowError(result.Errors);

            _labelScanEngineResult.Text = $"{result.Count} {TranslationServer.Translate("!godotfound")}";
        }
        EndScanUpdateUI();
    }

    private void ScanProjectsWorkerDoWork(object sender, DoWorkEventArgs e)
    {
        string errors = string.Empty;
        ProjectsConfig projectsConfig = ProjectsView?.Refresh(out errors);
        int count = projectsConfig.Projects.Count;
        e.Result = new ScanResult(count, errors);
    }

    private void ScanProjectsWorkerOnCompleted(object sender, RunWorkerCompletedEventArgs e)
    {
        if (e.Result is ScanResult result)
        {
            if (!string.IsNullOrEmpty(result.Errors))
                ErrorTools.ShowError(result.Errors);

            _labelScanProjectsResult.Text = $"{result.Count} {TranslationServer.Translate("!projectsfound")}";
        }
        EndScanUpdateUI();
    }
}
