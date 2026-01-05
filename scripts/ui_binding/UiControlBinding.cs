using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlBinding<T> : Control, IUiControlBinding where T : class
{
    public abstract T BindingContext { get; }

    private HashSet<PropertyChangedEvent> PropertyChangedEvents = [];

    public void RegisterPropertyChangedEvent(string propertyName, Action<object> action)
    {
        if (!HasProperty(propertyName)) throw new Exception($"Can't register unknown property '{propertyName}'");
        PropertyChangedEvents.Add(new PropertyChangedEvent(propertyName, action));
    }

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

        return prop.GetValue(BindingContext);
    }

    public V GetPropertyValue<V>(string propertyName)
    {
        return (V)GetPropertyValue(propertyName);
    }

    public virtual void SetPropertyValue(string propertyName, object propertyValue)
    {
        var type = BindingContext.GetType();

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

        SetPropValue(propertyValue, prop);
    }

    protected virtual void SetPropertyValue<TValue>(Expression<Func<T, TValue>> member, object propertyValue)
    {
        if (member is null) throw new ArgumentNullException(nameof(member));

        MemberExpression expr = member.Body switch
        {
            MemberExpression m => m,
            UnaryExpression u when u.Operand is MemberExpression m => m,
            _ => throw new ArgumentException("Expression must target a property")
        };

        if (expr.Member is not PropertyInfo prop)
            throw new ArgumentException("Expression must target a property");

        SetPropValue(propertyValue, prop);
    }

    private void SetPropValue(object propertyValue, PropertyInfo prop)
    {
        prop.SetValue(BindingContext, propertyValue);

        foreach (PropertyChangedEvent p in PropertyChangedEvents.Where(p => p.PropertyName == prop.Name))
        {
            p.Action.Invoke(propertyValue);
        }
    }

    private PropertyInfo GetPropertyInfo(string propertyName)
    {
        Type type = BindingContext.GetType();

        PropertyInfo prop = type.GetProperty(
            propertyName,
            BindingFlags.Instance |
            BindingFlags.Public |
            BindingFlags.NonPublic
        );
        return prop;
    }
}
