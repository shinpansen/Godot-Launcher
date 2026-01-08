using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

[GlobalClass]
public partial class PropertyBinding : Resource
{
    [Export]
    public StringName PropertyPath { get; set; }

    [Export]
    public string Binding { get; set; }

    [Export]
    public bool UseRegularExpression { get; set; }
}
