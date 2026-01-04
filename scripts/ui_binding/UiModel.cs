using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public class UiModel
{
    public bool HasProperty(string propertyName)
    {
        PropertyInfo prop = GetPropertyInfo(propertyName);
        return prop is not null;
    }

    public object GetPropertyValue(string propertyName)
    {
        PropertyInfo prop = GetPropertyInfo(propertyName);
        if (prop == null)
            throw new ArgumentException($"Property '{propertyName}' not found.");

        return prop.GetValue(this);
    }

    public T GetPropertyValue<T>(string propertyName)
    {
        return (T)GetPropertyValue(propertyName);
    }

    public void SetPropertyValue(string propertyName, object propertyValue)
    {
        var type = GetType();

        var prop = type.GetProperty(
            propertyName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );

        if (prop == null)
            throw new ArgumentException($"Property '{propertyName}' not found.");

        if (!prop.CanWrite)
            throw new InvalidOperationException($"Property '{propertyName}' has no setter.");

        prop.SetValue(this, propertyValue);
    }

    private PropertyInfo GetPropertyInfo(string propertyName)
    {
        Type type = this.GetType();

        PropertyInfo prop = type.GetProperty(
            propertyName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );
        return prop;
    }
}
