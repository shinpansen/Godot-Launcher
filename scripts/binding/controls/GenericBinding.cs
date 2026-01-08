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

[GlobalClass]
public partial class GenericBinding : Control
{
    [Export]
    [ExportGroup("Binding Override")]
    public Array<PropertyBinding> PropertiesBindings { get; set; }

    private IControlBinding _binding;

    private readonly List<PropertyEventInfo> _propertyEventInfos =
    [
        new PropertyEventInfo("toggled", "button_pressed", true),
        new PropertyEventInfo("text_changed", "text", false),
    ];

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

    private void BindPropertyValue(PropertyBinding prop)
    {
        if (prop.UseRegularExpression)
        {
            SetRegexPropertyValue(prop);
            RegexTools.ExtractMatchingValues(prop.Binding, BindingTools.BindingRegex)
                .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) => SetRegexPropertyValue(prop)));
        }
        else
        {
            object propValue = _binding.GetPropertyValue(prop.Binding);
            this.Set(prop.PropertyPath, VariantTools.FromCSharpObject(propValue));
            _binding.RegisterPropertyChangedEvent(prop.Binding, 
                (v) => this.Set(prop.PropertyPath, VariantTools.FromCSharpObject(v)));
        }
    }

    private void AttachSignals()
    {
        foreach(var pi in _propertyEventInfos)
        {
            if (!HasSignal(pi.SignalName)) continue;

            var boundProp = PropertiesBindings.FirstOrDefault(p => p.PropertyPath == pi.PropertyPath);
            if (boundProp is null) continue;

            var propValue = Get(pi.PropertyPath).Obj;
            if (propValue is null) continue;

            if(pi.HasArgs)
            {
                Connect(pi.SignalName, Callable.From((Variant v) =>
                {
                    _binding.SetPropertyValue(boundProp.Binding, Get(pi.PropertyPath).Obj, true);
                }));
            }
            else
            {
                Connect(pi.SignalName, Callable.From(() =>
                {
                    _binding.SetPropertyValue(boundProp.Binding, Get(pi.PropertyPath).Obj, true);
                }));
            }
        }
    }

    private void SetRegexPropertyValue(PropertyBinding prop)
    {
        var propValue = BindingTools.BindReplacedMatchingValues(prop.Binding, _binding);
        this.Set(prop.PropertyPath, VariantTools.FromCSharpObject(propValue));
    }
}
