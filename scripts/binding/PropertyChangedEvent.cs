using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

public struct PropertyChangedEvent(string propertyName, Action<object> action)
{
    public string PropertyName { get; set; } = propertyName;
    public Action<object> Action { get; set; } = action;
}
