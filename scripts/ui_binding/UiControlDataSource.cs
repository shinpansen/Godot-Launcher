using Godot;
using GodotLauncher.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlDataSource<T> : UiControlBinding<T>, IUiControlDataSource where T : class
{
    public override T BindingContext
    {
        get
        {
            if (_bindingContext is null)
                _bindingContext = LoadDataSource();
            return _bindingContext;
        }
    }

    private T _bindingContext;

    public override void SetPropertyValue(string propertyName, object propertyValue)
    {
        base.SetPropertyValue(propertyName, propertyValue);
        SaveDataSource();
    }

    protected override void SetPropertyValue<TValue>(Expression<Func<T, TValue>> member, object propertyValue)
    {
        base.SetPropertyValue(member, propertyValue);
        SaveDataSource();
    }

    public virtual void NotifyItemChanged()
    {
        SaveDataSource();
    }

    protected abstract T LoadDataSource();
    protected abstract void SaveDataSource();
}
