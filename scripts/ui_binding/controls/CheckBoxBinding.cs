using Godot;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.UiBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace GodotLauncher.Scripts.UiBinding.Controls;

[GlobalClass]
public partial class CheckBoxBinding : CheckBox
{
    [Export]
    public string BindingPropertyName { get; set; }

    private IUiControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IUiControlBinding binding) _binding = binding;
        else return;

        if (_binding.HasProperty(BindingPropertyName))
        {
            _binding.RegisterPropertyChangedEvent(BindingPropertyName, value => UpdateValue((bool)value));
            this.Toggled += value => _binding.SetPropertyValue(BindingPropertyName, value);

            bool checkedValue = _binding.GetPropertyValue<bool>(BindingPropertyName);
            UpdateValue(checkedValue);
        }
    }

    public void UpdateValue(bool value)
    {
        ButtonPressed = value;
    }
}
