using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Binding.Controls;
using GodotLauncher.Scripts.Extensions;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes.VersionsView;
using GodotLauncher.Scripts.UserData;
using System;
using System.Diagnostics;

namespace GodotLauncher.Scripts.Scenes.ProjectsView;

public partial class ProjectItemView : ItemBinding<Project>
{
    private Window _windowEdit => GetNode<Window>("%WindowEdit");
    private LaunchProjectSettings _launchProjectSettings => GetNode<LaunchProjectSettings>("%LaunchProjectSettings");
    private TextureRect _projectIcon => GetNode<TextureRect>("%ProjectIcon");

    public override void _Ready()
    {
        UpdateIcon();
    }

    private void OnButtonEditDown()
    {
        if (BindingContext.LaunchVersion is null)
        {
            GD.PrintErr("Can't edit project. No godot version available");
            return;
        }

        LaunchGodotEditor(
            BindingContext.LaunchVersion.Path,
            BindingContext.Path,
            BindingContext.LaunchArguments ?? "");

        Settings settings = UserDataLoader.LoadUserSettings();
        if (settings.CloseLauncherWhenStartingProject) GetTree().Quit();
    }

    private void OnLabelTypeGuiInput(InputEvent e)
    {
        if (e is InputEventMouseButton mb &&
            mb.Pressed &&
            mb.ButtonIndex == MouseButton.Left)
        {
            _windowEdit.Show();
            _launchProjectSettings.UpdateSettings(
                BindingContext.DefaultLaunchVersion,
                BindingContext.OptimalLaunchVersion,
                BindingContext.AvailableVersions, 
                BindingContext.LaunchArguments);
        }
    }

    private void HideWindowEdit()
    {
        _windowEdit.Hide();
    }

    private void SaveLaunchSettings(Node launchProjectSettings)
    {
        if (launchProjectSettings is LaunchProjectSettings settings)
        {
            SetPropertyValue(p => p.DefaultLaunchVersion, settings.SelectedVersion);
            SetPropertyValue(p => p.LaunchArguments, settings.Args);
            GetNode<GenericBinding>("%LabelType").Refresh();
            HideWindowEdit();
        }
        else
            GD.PrintErr("Can't save project launch settings. Invalid node type");
    }

    private void UpdateIcon()
    {
        if (string.IsNullOrEmpty(BindingContext.IconPath)) return;

        string iconPath = BindingContext.IconPath;
        if (iconPath.StartsWith(@"res://"))
            iconPath = @$"{BindingContext.Path}/{iconPath.Substring(6)}";
        _projectIcon.SetTextureFromPath(iconPath);
    }

    private void LaunchGodotEditor(
        string godotExePath,
        string projectPath,
        string extraArgs = "")
    {
        var startInfo = new ProcessStartInfo
        {
            FileName = godotExePath,
            Arguments = $"--editor --path \"{projectPath}\" {extraArgs}",
            UseShellExecute = false
        };

        Process.Start(startInfo);
    }
}
