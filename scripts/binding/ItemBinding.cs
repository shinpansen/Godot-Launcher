using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

public abstract partial class ItemBinding<T> : ControlBinding<T>, IItemBinding where T : class
{
    public override T BindingContext => _bindingContext;

    protected IDataSourceBinding DataSource = null;

    private T _bindingContext;

    public void Init(object model)
    {
        if (model is T context)
            _bindingContext = context;
        else
            throw new Exception($"Model must be of type {typeof(T).FullName}");
    }

    public void Init(object model, IDataSourceBinding dataSource)
    {
        DataSource = dataSource;
        Init(model);
    }

    public override void SetPropertyValue(
        string propertyName, 
        object propertyValue, 
        bool disablePropertyChangedEvents = false)
    {
        base.SetPropertyValue(propertyName, propertyValue, disablePropertyChangedEvents);
        DataSource?.NotifyItemChanged();
    }

    protected override void SetPropertyValue<TValue>(
        Expression<Func<T, TValue>> member, 
        object propertyValue,
        bool disablePropertyChangedEvents = false)
    {
        base.SetPropertyValue(member, propertyValue, disablePropertyChangedEvents);
        DataSource?.NotifyItemChanged();
    }
}
