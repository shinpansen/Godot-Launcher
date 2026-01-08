using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

public struct PropertyEventInfo(string signalName, string propertyPath, bool hasArgs)
{
    public string SignalName { get; set; } = signalName;
    public string PropertyPath { get; set; } = propertyPath;
    public bool HasArgs { get; set; } = hasArgs;
}
