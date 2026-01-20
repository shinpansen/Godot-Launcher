using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Scenes.Components;

[GlobalClass]
public partial class Loader : TextureRect
{
    [Export]
    public float Speed { get; set; } = 4f;

    [Export]
    public Vector2 PivotOffsetOverride { get; set; }

    public override void _Ready()
    {
        PivotOffset = PivotOffsetOverride;
    }

    public override void _Process(double delta)
    {
        if (!Visible) return;
        Rotation += (float)(delta * Speed);
    }
}
