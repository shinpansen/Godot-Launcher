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
    public T BindingContext { get; protected set; }

    private Dictionary<ulong, bool> RegisteredControls = [];

    public virtual void Init(T model)
    {
        this.BindingContext = model;
    }

    public void RegisterControl(ulong controlId)
    {
        RegisteredControls.Add(controlId, false);
    }

    public bool HasChanged(ulong controlId)
    {
        if(RegisteredControls.ContainsKey(controlId))
        { 
            var hasChanged = RegisteredControls[controlId];
            RegisteredControls[controlId] = false;
            return hasChanged;
        }
        return false;
    }

    public object GetPropertyValue(string propertyName) => BindingContext.GetPropertyValue(propertyName);
    public V GetPropertyValue<V>(string propertyName) => BindingContext.GetPropertyValue<V>(propertyName);

    protected void SetPropertyValue<TValue>(Expression<Func<T, TValue>> member, object propertyValue)
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

        foreach (var key in RegisteredControls.Keys.ToList()) RegisteredControls[key] = true;
        prop.SetValue(BindingContext, propertyValue);
    }
}
