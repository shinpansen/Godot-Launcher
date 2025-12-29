using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;

namespace GodotLauncher.Scripts.Scenes.EnginesView;

public partial class EngineItemView : Control
{
    private EngineVersion _version;

    private TextureRect _icon => GetNode<TextureRect>("%Icon");
    private Label _versionLabel => GetNode<Label>("%LabelVersion");
    private Label _fileNameLabel => GetNode<Label>("%LabelFileName");

    public void Init(EngineVersion item)
    {
        _version = item;
    }

    public override void _Ready()
    {
        if(_version.CustomIcon is not null)
        {
            var mat = (ShaderMaterial)_icon.Material;
            mat.SetShaderParameter("background", _version.CustomIcon.Background);
            mat.SetShaderParameter("gray_scale", _version.CustomIcon.GrayScale);
            Color color = Color.FromString($"#{_version.CustomIcon.HexColor}", Colors.White);
            mat.SetShaderParameter("tint_color", color);
        }

        _versionLabel.Text = _version.Version;
        _fileNameLabel.Text = _version.Name.Length > 25 ? _version.Name.Substring(0, 22) + "..." : _version.Name;
        _fileNameLabel.TooltipText = _version.Name;
    }

    private void OnButtonLaunchDown()
    {
        SystemTools.OpenFileExplorer(_version.Path); //TODO handle linux and mac os
        GetTree().Quit();
    }

    private void OnButtonFolderDown()
    {
        string directoryPath = System.IO.Path.GetDirectoryName(_version.Path);
        SystemTools.OpenFileExplorer(directoryPath);
    }
}