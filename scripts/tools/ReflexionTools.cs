using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static Godot.RenderingServer;

namespace GodotLauncher.Scripts.Tools;

public class ReflexionTools
{
    public static bool HasProperty<T>(T instance, string propertyName) where T : class
    {
        PropertyInfo prop = GetPropertyInfo(instance, propertyName);
        return prop is not null;
    }

    public static PropertyInfo GetPropertyInfo<T>(T instance, string propertyName)
    {
        Type type = instance.GetType();
        PropertyInfo prop = type.GetProperty(
            propertyName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );
        return prop;
    }

    public static object GetPropertyValue<T>(T instance, string propertyName)
    {
        PropertyInfo prop = GetPropertyInfo(instance, propertyName);
        if (prop == null)
            throw new ArgumentException($"Property '{propertyName}' not found in type {instance.GetType().FullName}.");

        return prop.GetValue(instance);
    }

    public static V GetPropertyValue<V, T>(T instance, string propertyName)
    {
        return (V)GetPropertyValue(instance, propertyName);
    }

    public static void SetPropertyValue<T>(T instance, string propertyName, T propertyValue)
    {
        var prop = GetPropertyInfo(instance, propertyName);

        if (prop == null)
            throw new ArgumentException($"Property '{propertyName}' not found.");

        if (!prop.CanWrite)
            throw new InvalidOperationException($"Property '{propertyName}' has no setter.");

        prop.SetValue(instance, propertyValue);
    }
}
