using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UiBinding;

public abstract partial class UiControlBinding<T> : Control where T : UiModel
{
    public UiModel BindingContext { get; protected set; }

    public abstract void Init(T model);
}
