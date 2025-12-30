using Godot;
using GodotLauncher.Scripts.Scenes.VersionsView;
using System;

namespace GodotLauncher.Scripts.Scenes;

public partial class Main : Control
{
    private TabContainer _sideBarTabContainer => GetNode<TabContainer>("%Content");

    private void OnButtonVersionsDown() => ChangeTab(0);

    private void OnButtonProjectsDown() => ChangeTab(1);

    private void OnButtonSettingsDown() => ChangeTab(2);

    private void ChangeTab(int index) => _sideBarTabContainer.CurrentTab = index;
}
