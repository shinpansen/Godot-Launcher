using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlItem<T> : UiControlBinding<T>, IUiControlItem where T : class
{
    public override T BindingContext => _bindingContext;

    protected IUiControlDataSource DataSource = null;

    private T _bindingContext;

    public void Init(object model)
    {
        if (model is T context)
            _bindingContext = context;
        else
            throw new Exception($"Model must be of type {typeof(T).FullName}");
    }

    public void Init(object model, IUiControlDataSource dataSource)
    {
        DataSource = dataSource;
        Init(model);
    }

    public override void SetPropertyValue(string propertyName, object propertyValue)
    {
        base.SetPropertyValue(propertyName, propertyValue);
        DataSource?.NotifyItemChanged();
    }

    protected override void SetPropertyValue<TValue>(Expression<Func<T, TValue>> member, object propertyValue)
    {
        base.SetPropertyValue(member, propertyValue);
        DataSource?.NotifyItemChanged();
    }
}
