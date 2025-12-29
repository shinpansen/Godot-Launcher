using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes;
using GodotLauncher.Scripts.UserData;
using System;

namespace GodotLauncher.Scripts.Scenes.EnginesView;

public partial class EnginesView : Control
{
    public override void _Ready()
    {
        VersionsConfig config = UserDataLoader.LoadUserEngines("versions.json");
        var engines = UserDataScanner.ScanUserEngines(@"C:\Program Files (x86)\Godot");
        config.Versions = engines;

        var grid = GetNode<HFlowContainer>("%EnginesContainer");
        config.Versions.ForEach(e =>
        {
            PackedScene engineItemScene = GD.Load<PackedScene>("res://scenes/components/engine_item.tscn");
            var item = engineItemScene.Instantiate<EngineItemView>();
            //e.CustomIcon = new CustomIcon() { GrayScale = true, HexColor = "ff0000" };
            item.Init(e);
            grid.AddChild(item);
        });

        //using var file = FileAccess.Open("user://engines.json", FileAccess.ModeFlags.Write);

        //var item = new EngineItem("4.2.1", @"C:\Program Files (x86)\Godot\Godot_v4.2.1-stable_mono_win64\Godot_v4.2.1-stable_mono_win64.exe");
        //var item2 = new EngineItem("4.4.1", @"C:\Program Files (x86)\Godot\Godot_v4.4.1-stable_mono_win64\Godot_v4.4.1-stable_mono_win64.exe");
        //var items = new EnginesList()
        //{
        //    Engines = new System.Collections.Generic.List<EngineItem> { item, item2 },
        //};

        //file.StoreString(System.Text.Json.JsonSerializer.Serialize(items));
    }
}
