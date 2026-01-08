using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Binding;

public abstract partial class DataSourceBinding<T> : ControlBinding<T>, IDataSourceBinding where T : class
{
    [Export]
    [ExportCategory("Save")]
    public string ConfigFileName { get; set; }

    [Export]
    public bool AutoSaveConfig { get; set; } = true;

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

    public override void SetPropertyValue(
        string propertyName, 
        object propertyValue,
        bool disablePropertyChangedEvents = false)
    {
        base.SetPropertyValue(propertyName, propertyValue, disablePropertyChangedEvents);
        if (AutoSaveConfig) SaveDataSource();
    }

    protected override void SetPropertyValue<TValue>(
        Expression<Func<T, TValue>> member,
        object propertyValue,
        bool disablePropertyChangedEvents = false)
    {
        base.SetPropertyValue(member, propertyValue, disablePropertyChangedEvents);
        if(AutoSaveConfig) SaveDataSource();
    }

    public virtual void NotifyItemChanged()
    {
        if(AutoSaveConfig) SaveDataSource();
    }

    protected abstract T LoadDataSource();

    protected virtual void SaveDataSource()
    {
        if (!string.IsNullOrEmpty(ConfigFileName))
        {
            UserDataLoader.SaveConfig(ConfigFileName, BindingContext);
        }
    }
}
