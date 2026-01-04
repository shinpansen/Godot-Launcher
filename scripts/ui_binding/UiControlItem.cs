using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlItem<T> : UiControlBinding<T>, IUiControlItem where T : UiModel
{
    public override T BindingContext => _bindingContext;

    private T _bindingContext;

    public void Init(T bindingContext)
    {
        _bindingContext = bindingContext;
    }

    public void Init(UiModel model)
    {
        if (model is T context)
            _bindingContext = context;
        else
            throw new Exception($"BindingContext must be of type {typeof(T).FullName}");
    }
}
