using Godot;
using Godot.Collections;
using GodotLauncher.Scripts.Binding.Interfaces;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace GodotLauncher.Scripts.Binding.Controls;

public partial class GenericBinding : Control
{
    [Export]
    [ExportGroup("Binding Override")]
    public Array<ResourceBinding> ResourcesBindings { get; set; }

    [Export]
    public Array<SignalBinding> SignalsBindings =
    [
        new SignalBinding("toggled", "button_pressed", true, true),
        new SignalBinding("text_changed", "text", false, true),
    ];

    private IControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IControlBinding binding) _binding = binding;
        else return;

        if (ResourcesBindings is null || ResourcesBindings.Count == 0) return;
        foreach (var prop in ResourcesBindings)
        {
            try
            {
                BindPropertyValue(prop);
            }
            catch (System.Exception ex)
            {
                GD.PrintErr($"Can't bind property '{prop.PropertyPath}' : {ex.Message}");
            }
        }
        AttachSignals();
    }

    public void Refresh()
    {
        if (ResourcesBindings is null || ResourcesBindings.Count == 0) return;
        foreach (var res in ResourcesBindings)
        {
            if (res is ExpressionBinding exprBinding)
            {
                SetRegexPropertyValue(exprBinding);
            }
            else if (res is PropertyBinding propBinding)
            {
                object propValue = _binding.GetPropertyValue(propBinding.BindingName);
                SetNodePropertyValue(res.PropertyPath, propValue);
            }
        }
    }

    private void BindPropertyValue(ResourceBinding resourceBinding)
    {
        if (resourceBinding is ExpressionBinding exprBinding)
        {
            SetRegexPropertyValue(exprBinding);
            BindingTools.ExtractBindingValues(exprBinding.Expression)
                .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) =>
                {
                    SetRegexPropertyValue(exprBinding);
                }));
        }
        else if (resourceBinding is PropertyBinding propBinding)
        {
            object propValue = _binding.GetPropertyValue(propBinding.BindingName);
            SetNodePropertyValue(propBinding.PropertyPath, propValue);
            _binding.RegisterPropertyChangedEvent(propBinding.BindingName, (v) =>
            {
                SetNodePropertyValue(propBinding.PropertyPath, v); 
            });
        }
    }

    private void SetNodePropertyValue(string propertyPath, object propValue)
    {
        if(propertyPath.StartsWith("shader_parameter/"))
        {
            string shaderPathValue = propertyPath.Split('/')[1];
            var mat = (ShaderMaterial)this.Material;
            if (mat is not null)
                mat.SetShaderParameter(shaderPathValue, VariantTools.FromCSharpObject(propValue));
        }
        else
        {
            //TODO : Find a better way to prevent disrupting text input
            foreach(var sb in SignalsBindings.Where(s => s.PreventValuedChangedWhenFocused))
            {
                if (HasSignal(sb.SignalPath) && HasFocus()) return;
            }

            Variant currentValue = this.Get(propertyPath);
            if(!propValue?.Equals(currentValue.Obj) ?? true)
                this.Set(propertyPath, VariantTools.FromCSharpObject(propValue));
        }
    }

    private void SetRegexPropertyValue(ExpressionBinding exprBinding)
    {
        var propValue = BindingTools.BindReplacedMatchingValues(exprBinding.Expression, _binding);
        this.Set(exprBinding.PropertyPath, VariantTools.FromCSharpObject(propValue));
    }

    private void AttachSignals()
    {
        foreach(var pi in SignalsBindings)
        {
            if (!HasSignal(pi.SignalPath)) continue;

            var boundProp = ResourcesBindings.FirstOrDefault(p => p.PropertyPath == pi.PropertyPath);
            if (boundProp is null || boundProp is not PropertyBinding propBinding) continue;

            var propValue = Get(pi.PropertyPath).Obj;
            if (propValue is null) continue;

            if(pi.HasArgs)
            {
                Connect(pi.SignalPath, Callable.From((Variant v) =>
                {
                    _binding.SetPropertyValue(propBinding.BindingName, Get(pi.PropertyPath).Obj, false);
                }));
            }
            else
            {
                Connect(pi.SignalPath, Callable.From(() =>
                {
                    _binding.SetPropertyValue(propBinding.BindingName, Get(pi.PropertyPath).Obj, false);
                }));
            }
        }
    }
}
