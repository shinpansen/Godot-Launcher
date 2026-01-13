using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

[GlobalClass]
public partial class ExpressionBinding : ResourceBinding
{
    [Export(PropertyHint.MultilineText)]
    public string Expression { get; set; }
}
