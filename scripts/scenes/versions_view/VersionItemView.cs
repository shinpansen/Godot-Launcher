using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.UiBinding;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionItemView : UiControlItem<EngineVersion>
{
    private TextureRect _icon => GetNode<TextureRect>("%Icon");

    public override void _Ready()
    {
        if(BindingContext.CustomIcon is not null)
        {
            var mat = (ShaderMaterial)_icon.Material;
            mat.SetShaderParameter("background", BindingContext.CustomIcon.Background);
            mat.SetShaderParameter("gray_scale", BindingContext.CustomIcon.GrayScale);
            Color color = Color.FromString($"#{BindingContext.CustomIcon.HexColor}", Colors.White);
            mat.SetShaderParameter("tint_color", color);
        }
    }

    private void OnButtonLaunchDown()
    {
        SystemTools.OpenFileExplorer(BindingContext.Path); //TODO handle linux and mac os
        GetTree().Quit();
    }

    private void OnButtonFolderDown()
    {
        string directoryPath = System.IO.Path.GetDirectoryName(BindingContext.Path);
        SystemTools.OpenFileExplorer(directoryPath);
    }
}