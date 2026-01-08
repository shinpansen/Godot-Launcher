using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionItemView : ItemBinding<EngineVersion>
{
    private TextureRect _icon => GetNode<TextureRect>("%Icon");

    private void OnButtonLaunchDown()
    {
        //SetPropertyValue(e => e.Version, "666");
        SystemTools.OpenFileExplorer(BindingContext.Path); //TODO handle linux and mac os
        Settings settings = UserDataLoader.LoadUserSettings();
        if (settings.CloseLauncherWhenStartingGodot) GetTree().Quit();
    }

    private void OnButtonFolderDown()
    {
        string directoryPath = System.IO.Path.GetDirectoryName(BindingContext.Path);
        SystemTools.OpenFileExplorer(directoryPath);
    }
}