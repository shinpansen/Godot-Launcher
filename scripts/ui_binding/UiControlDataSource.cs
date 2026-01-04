using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlDataSource<T> : UiControlBinding<T> where T : UiModel
{
    public override T BindingContext
    {
        get
        {
            if (_bindingContext is null)
                _bindingContext = LoadBindingContext();
            return _bindingContext;
        }
    }

    private T _bindingContext;

    protected abstract T LoadBindingContext();
}
