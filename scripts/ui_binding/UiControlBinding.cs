using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlBinding<T> : Control, IUiControlBinding where T : UiModel
{
    public T BindingContext { get; protected set; }

    public virtual void Init(T model)
    {
        this.BindingContext = model;
    }

    public object GetPropertyValue(string propertyName) => BindingContext.GetPropertyValue(propertyName);
}
