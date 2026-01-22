using Godot;
using GodotLauncher.Scripts.Scenes.VersionsView;
using GodotLauncher.Scripts.Scenes.SettingsView;
using System;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.UserData;

namespace GodotLauncher.Scripts.Scenes;

public partial class Main : Control
{
    private Window _windowError => GetNode<Window>("%WindowError");
    private Window _windowInstall => GetNode<Window>("%WindowInstall");
    private DownloadsView.DownloadsView _downloadsView => GetNode<DownloadsView.DownloadsView>("%DownloadsView");
    private TabContainer _sideBarTabContainer => GetNode<TabContainer>("%Content");

    private Button[] _buttons;

    public override void _Ready()
    {
        DialogTools.ErrorWindow = _windowError;

        _buttons = new Button[]
        {
            GetNode<Button>("%ButtonVersions"),
            GetNode<Button>("%ButtonProjects"),
            GetNode<Button>("%ButtonSettings"),
        };
        Models.Settings settings = UserDataLoader.LoadUserSettings();
        ChangeTab((int)settings.TabSelect);
    }

    private void CloseErrorWindow()
    {
        _windowError.Hide();
    }

    private void OnButtonVersionsDown() => ChangeTab(0);

    private void OnButtonProjectsDown() => ChangeTab(1);

    private void OnButtonSettingsDown() => ChangeTab(2);

    private void ChangeTab(int index)
    {
        _buttons[index].ButtonPressed = true;
        _sideBarTabContainer.CurrentTab = index;
    }

    private void OnButtonInstallDown()
    {
        _windowInstall.Show();
        _downloadsView.Load();
    }

    private void HideInstallWindow()
    {
        _windowInstall.Hide();
    }
}
