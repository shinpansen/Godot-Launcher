using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Extensions;

public static class NodeExtensions
{
    public static void Clear(this Node node)
    {
        foreach (Node child in node.GetChildren())
        {
            child.QueueFree();
        }
    }
}
