

using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

[GlobalClass]
public abstract partial class ResourceBinding : Resource
{
    [Export]
    public StringName PropertyPath { get; set; }
}
