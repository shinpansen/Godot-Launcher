using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public class PropertyChangedEvent
{
    public string PropertyName { get; set; }
    public Action<object> Action { get; set; }

    public PropertyChangedEvent(string propertyName, Action<object> action)
    {
        PropertyName = propertyName;
        Action = action;
    }
}
