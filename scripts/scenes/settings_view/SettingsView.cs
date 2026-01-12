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

namespace GodotLauncher.Scripts.Scenes.SettingsView;

public partial class SettingsView : DataSourceBinding<Settings>
{
    [Export]
    public VersionsView.VersionsView VersionsView { get; set; }

    [Export]
    public ProjectsView.ProjectsView ProjectsView { get; set; }

    protected override Settings LoadDataSource() => UserDataLoader.LoadUserSettings();

    private FileDialog _fileDialogCustomPaths => GetNode<FileDialog>("%FileDialogCustomPaths");
    private FileDialog _fileDialogProjects => GetNode<FileDialog>("%FileDialogProjects");

    private void OnButtonScanEnginesDown()
    {
        //Todo - thread
        VersionsView?.Refresh();
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

    private void OnInstallDirSelected(string dirPath)
    {
        var paths = BindingContext.CustomInstallsDirectories;
        paths.Add(new FileSystemPath(dirPath));
        SetPropertyValue(s => s.CustomInstallsDirectories, paths);
    }

    private void OnProjectDirSelected(string dirPath)
    {
        var paths = BindingContext.ProjectsDirectories;
        paths.Add(new FileSystemPath(dirPath));
        SetPropertyValue(s => s.ProjectsDirectories, paths);
    }

    private void DeleteInstallDirectory(Node node)
    {
        if (node is ItemBinding<FileSystemPath> item)
        {
            if (BindingContext.CustomInstallsDirectories.Contains(item.BindingContext))
            {
                var paths = BindingContext.CustomInstallsDirectories;
                paths.Remove(item.BindingContext);
                SetPropertyValue(s => s.CustomInstallsDirectories, paths);
            }
        }
    }

    private void DeleteProjectDirectory(Node node)
    {
        if (node is ItemBinding<FileSystemPath> item)
        {
            if (BindingContext.ProjectsDirectories.Contains(item.BindingContext))
            {
                var paths = BindingContext.ProjectsDirectories;
                paths.Remove(item.BindingContext);
                SetPropertyValue(s => s.ProjectsDirectories, paths);
            }
        }
    }
}
