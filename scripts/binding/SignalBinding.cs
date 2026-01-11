using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

[GlobalClass]
public partial class SignalBinding : Resource
{
    [Export]
    public string SignalPath { get; set; }

    [Export]
    public StringName PropertyPath { get; set; }

    [Export]
    public bool HasArgs { get; set; }

    [Export]
    public bool PreventValuedChangedWhenFocused { get; set; } = true;

    public SignalBinding()
    {
    }

    public SignalBinding(
        string signalPath, 
        StringName propertyPath, 
        bool hasArgs,
        bool preventValuedChangedWhenFocused = true)
    {
        SignalPath = signalPath;
        PropertyPath = propertyPath;
        HasArgs = hasArgs;
        PreventValuedChangedWhenFocused = preventValuedChangedWhenFocused;
    }
}
