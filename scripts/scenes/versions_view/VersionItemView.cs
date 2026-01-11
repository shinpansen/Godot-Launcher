using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using GodotLauncher.Scripts.Binding.Controls;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionItemView : ItemBinding<EngineVersion>
{
    private TextureRect _icon => GetNode<TextureRect>("%Icon");
    private Window _windowEdit => GetNode<Window>("%WindowEdit");
    private IconCustomization _iconCustomization =>  GetNode<IconCustomization>("%IconCustomization");

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

    private void OnButtonEditDown()
    {
        _windowEdit.Show();
        _iconCustomization.UpdateSettings(BindingContext.CustomIcon);
    }

    private void HideWindowEdit()
    {
        _windowEdit.Hide();
    }

    private void SaveCustomIcon(Node iconCustomization)
    {
        if (iconCustomization is IconCustomization ic)
        {
            SetPropertyValue(v => v.CustomIcon, new CustomIcon()
            {
                Background = ic.BindingContext.Background,
                GrayScale = ic.BindingContext.GrayScale,
                HexColor = ic.BindingContext.HexColor,
                Color = ic.BindingContext.Color
            });
            GetNode<GenericBinding>("%Icon").Refresh();
            HideWindowEdit();
        }
        else
            GD.PrintErr("Can't save custom icon. Invalid node type");
    }
}