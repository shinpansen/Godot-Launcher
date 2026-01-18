using Godot;
using GodotLauncher.Scripts.Scenes.VersionsView;
using GodotLauncher.Scripts.Scenes.SettingsView;
using System;
using GodotLauncher.Scripts.Tools;

namespace GodotLauncher.Scripts.Scenes;

public partial class Main : Control
{
    private Window _windowError => GetNode<Window>("%WindowError");
    private TabContainer _sideBarTabContainer => GetNode<TabContainer>("%Content");

    public override void _Ready()
    {
        ErrorTools.ErrorWindow = _windowError;
    }

    private void CloseErrorWindow()
    {
        _windowError.Hide();
    }

    private void OnButtonVersionsDown() => ChangeTab(0);

    private void OnButtonProjectsDown() => ChangeTab(1);

    private void OnButtonSettingsDown() => ChangeTab(2);

    private void ChangeTab(int index) => _sideBarTabContainer.CurrentTab = index;
}
