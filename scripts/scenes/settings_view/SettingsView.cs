using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Godot;
using GodotLauncher.Scripts.Scenes.VersionsView;
using GodotLauncher.Scripts.Enums;

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

    public override void _Ready()
    {
        _scrollContainer.ClipContents = true;
        OptionButtonLanguagesItemSelected((long)BindingContext.Language);
    }

    public void AddExcludedFile(string path)
    {
        OnExcludedFilesFileSelected(new Godot.Collections.Array<string>() { path });
    }

    private void OptionButtonLanguagesItemSelected(long id)
    {
        Language language = (Language)id;
        switch (language)
        {
            case Language.Default:
                string culture = System.Globalization.CultureInfo.InstalledUICulture.Name;
                TranslationServer.SetLocale(culture.Split('-').First());
                break;
            case Language.English:
                TranslationServer.SetLocale("en");
                break;
            case Language.French:
                TranslationServer.SetLocale("fr");
                break;
            case Language.German:
                TranslationServer.SetLocale("de");
                break;
            case Language.Spanish:
                TranslationServer.SetLocale("es");
                break;
            case Language.Italian:
                TranslationServer.SetLocale("it");
                break;
            case Language.Portugese:
                TranslationServer.SetLocale("pt");
                break;
            case Language.Russian:
                TranslationServer.SetLocale("ru");
                break;
            case Language.Japanese:
                TranslationServer.SetLocale("ja");
                break;
            case Language.Chinese:
                TranslationServer.SetLocale("zh");
                break;
            default:
                break;
        }
    }

    private void OnButtonScanEnginesDown()
    {
        //Todo - thread
        VersionsConfig versionsConfig = VersionsView?.Refresh();
        ProjectsView?.RefreshProjectsVersions(versionsConfig.Versions);
    }

    private void OnButtonScanProjectsDown()
    {
        //Todo - thread
        ProjectsView?.Refresh();
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
}
