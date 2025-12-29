using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public class UiModel
{
    public object GetPropertyValue(string propertyName)
    {
        var type = this.GetType();

        var prop = type.GetProperty(
            propertyName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );

        if (prop == null)
            throw new ArgumentException($"Property '{propertyName}' not found.");

        return prop.GetValue(this);
    }

    public T GetPropertyValue<T>(string propertyName)
    {
        var prop = GetType().GetProperty(
            propertyName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );

        if (prop == null)
            throw new ArgumentException($"Property '{propertyName}' not found.");

        return (T)prop.GetValue(this);
    }
}
