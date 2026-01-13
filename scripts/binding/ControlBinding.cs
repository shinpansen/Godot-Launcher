using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

public abstract partial class ControlBinding<T> : Control, IControlBinding where T : class
{
    public abstract T BindingContext { get; }

    private List<PropertyChangedEvent> PropertyChangedEvents = [];

    public object GetBindingContext()
    {
        return BindingContext;
    }

    public void RegisterPropertyChangedEvent(string propertyName, Action<object> action)
    {
        if (!HasProperty(propertyName)) throw new Exception($"Can't register unknown property '{propertyName}'");
        PropertyChangedEvents.Add(new PropertyChangedEvent(propertyName, action));
    }

    public bool HasProperty(string propertyName)
    {
        return ReflexionTools.HasProperty(BindingContext, propertyName);
    }

    public Type GetPropertyType(string propertyName)
    {
        return ReflexionTools.GetPropertyType(BindingContext, propertyName);
    }

    public object GetPropertyValue(string propertyName)
    {
        return ReflexionTools.GetPropertyValue(BindingContext, propertyName);
    }

    public V GetPropertyValue<V>(string propertyName)
    {
        return (V)GetPropertyValue(propertyName);
    }

    public virtual void SetPropertyValue(
        string propertyName, 
        object propertyValue, 
        bool disablePropertyChangedEvents = false)
    {
        ReflexionTools.SetPropertyValue(BindingContext, propertyName, propertyValue);
        if(!disablePropertyChangedEvents) InvokePropertyChangedEvents(propertyValue, propertyName);
    }

    protected virtual void SetPropertyValue<TValue>(
        Expression<Func<T, TValue>> member, 
        object propertyValue,
        bool disablePropertyChangedEvents = false)
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

        prop.SetValue(BindingContext, propertyValue);
        if (!disablePropertyChangedEvents) InvokePropertyChangedEvents(propertyValue, prop.Name);
    }

    private void InvokePropertyChangedEvents(object propertyValue, string propertyName)
    {
        foreach (PropertyChangedEvent p in PropertyChangedEvents.Where(p => p.PropertyName == propertyName))
        {
            p.Action.Invoke(propertyValue);
        }
    }
}
