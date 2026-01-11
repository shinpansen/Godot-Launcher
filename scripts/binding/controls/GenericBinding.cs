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
    public Array<PropertyBinding> PropertiesBindings { get; set; }

    [Export]
    public Array<SignalBinding> SignalsBindings =
    [
        new SignalBinding("toggled", "button_pressed", true, true),
        new SignalBinding("text_changed", "text", false, true),
    ];

    private IControlBinding _binding;
    private bool _preventCallbackLoop;

    public override void _Ready()
    {
        if (GetOwner() is IControlBinding binding) _binding = binding;
        else return;

        if (PropertiesBindings is null || PropertiesBindings.Count == 0) return;
        foreach (var prop in PropertiesBindings)
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
        if (PropertiesBindings is null || PropertiesBindings.Count == 0) return;
        foreach (var prop in PropertiesBindings)
        {
            _preventCallbackLoop = MustPreventCallbackLoop(prop.PropertyPath);
            if (prop.UseRegularExpression)
            {
                SetRegexPropertyValue(prop);
            }
            else
            {
                object propValue = _binding.GetPropertyValue(prop.Binding);
                SetNodePropertyValue(prop.PropertyPath, propValue);
            }
        }
    }

    private void BindPropertyValue(PropertyBinding prop)
    {
        if (prop.UseRegularExpression)
        {
            SetRegexPropertyValue(prop);
            RegexTools.ExtractMatchingValues(prop.Binding, BindingTools.BindingRegex)
                .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) =>
                {
                    _preventCallbackLoop = MustPreventCallbackLoop(prop.PropertyPath);
                    SetRegexPropertyValue(prop);
                }));
        }
        else
        {
            object propValue = _binding.GetPropertyValue(prop.Binding);
            SetNodePropertyValue(prop.PropertyPath, propValue);
            _binding.RegisterPropertyChangedEvent(prop.Binding, (v) =>
            {
                _preventCallbackLoop = MustPreventCallbackLoop(prop.PropertyPath);
                SetNodePropertyValue(prop.PropertyPath, v); 
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
            if(!propValue.Equals(currentValue.Obj))
                this.Set(propertyPath, VariantTools.FromCSharpObject(propValue));
        }
    }

    private void SetRegexPropertyValue(PropertyBinding prop)
    {
        var propValue = BindingTools.BindReplacedMatchingValues(prop.Binding, _binding);
        this.Set(prop.PropertyPath, VariantTools.FromCSharpObject(propValue));
    }

    private void AttachSignals()
    {
        foreach(var pi in SignalsBindings)
        {
            if (!HasSignal(pi.SignalPath)) continue;

            var boundProp = PropertiesBindings.FirstOrDefault(p => p.PropertyPath == pi.PropertyPath);
            if (boundProp is null) continue;

            var propValue = Get(pi.PropertyPath).Obj;
            if (propValue is null) continue;

            if(pi.HasArgs)
            {
                Connect(pi.SignalPath, Callable.From((Variant v) =>
                {
                    if(true)//!_preventCallbackLoop)
                        _binding.SetPropertyValue(boundProp.Binding, Get(pi.PropertyPath).Obj, false);
                    _preventCallbackLoop = false;
                }));
            }
            else
            {
                Connect(pi.SignalPath, Callable.From(() =>
                {
                    if (true)//!_preventCallbackLoop)
                        _binding.SetPropertyValue(boundProp.Binding, Get(pi.PropertyPath).Obj, false);
                    _preventCallbackLoop = false;
                }));
            }
        }
    }

    private bool MustPreventCallbackLoop(StringName propertyPath)
    {
        var signal = SignalsBindings.FirstOrDefault(s => s.PropertyPath == propertyPath);
        if (signal is not null && HasSignal(signal.SignalPath))
            return true;
        return false;
    }
}
