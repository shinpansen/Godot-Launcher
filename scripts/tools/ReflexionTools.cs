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
    public static bool HasProperty(object instance, string propertyName)
    {
        PropertyInfo propInfo = GetPropertyInfo(ref instance, propertyName);
        return propInfo is not null;
    }

    public static object GetPropertyValue(object instance, string propertyName)
    {
        PropertyInfo propInfo = GetPropertyInfo(ref instance, propertyName);
        if (propInfo == null)
            throw new ArgumentException($"Property '{propertyName}' not found in type {instance.GetType().FullName}.");

        return propInfo.GetValue(instance);
    }

    public static V GetPropertyValue<V, T>(T instance, string propertyName)
    {
        return (V)GetPropertyValue(instance, propertyName);
    }

    public static void SetPropertyValue(object instance, string propertyName, object propertyValue)
    {
        var propInfo = GetPropertyInfo(ref instance, propertyName);

        if (propInfo == null)
            throw new ArgumentException($"Property '{propertyName}' not found.");

        if (!propInfo.CanWrite)
            throw new InvalidOperationException($"Property '{propertyName}' has no setter.");

        propInfo.SetValue(instance, propertyValue);
    }

    public static PropertyInfo GetPropertyInfo(ref object instance, string propertyName)
    {
        var type = instance.GetType();
        if(propertyName.Contains('.'))
        {
            List<string> properties = propertyName.Split('.').ToList();
            PropertyInfo propInfo = null;
            while (properties.Count > 0)
            {
                string prop = properties[0];
                propInfo = GetPropertyInfo(type, prop);
                if (propInfo is not null)
                {
                    properties.RemoveAt(0);
                    if (properties.Count > 0)
                    {
                        type = propInfo.PropertyType;
                        instance = propInfo.GetValue(instance);
                    }
                }
                else 
                    return propInfo;
            }
            return propInfo;
        }
        else
            return GetPropertyInfo(type, propertyName);
    }

    private static PropertyInfo GetPropertyInfo(Type type, string propertyName)
    {
        PropertyInfo prop = type.GetProperty(
            propertyName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );
        return prop;
    }
}
