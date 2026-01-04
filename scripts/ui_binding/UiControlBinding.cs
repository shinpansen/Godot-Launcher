using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlBinding<T> : Control, IUiControlBinding where T : UiModel
{
    public abstract T BindingContext { get; }

    private HashSet<PropertyChangedEvent> PropertyChangedEvents = [];

    public void RegisterPropertyChangedEvent(string propertyName, Action<object> action)
    {
        if (!HasProperty(propertyName)) throw new Exception($"Can't register unknown property '{propertyName}'");
        PropertyChangedEvents.Add(new PropertyChangedEvent(propertyName, action));
    }

    public bool HasProperty(string propertyName) => BindingContext.HasProperty(propertyName);
    public object GetPropertyValue(string propertyName) => BindingContext.GetPropertyValue(propertyName);
    public V GetPropertyValue<V>(string propertyName) => BindingContext.GetPropertyValue<V>(propertyName);

    public virtual void SetPropertyValue(string propertyName, object propertyValue)
    {
        BindingContext.SetPropertyValue(propertyName, propertyValue);
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

        prop.SetValue(BindingContext, propertyValue);

        foreach (PropertyChangedEvent p in PropertyChangedEvents.Where(p => p.PropertyName == prop.Name))
        {
            p.Action.Invoke(propertyValue);
        }
    }
}
