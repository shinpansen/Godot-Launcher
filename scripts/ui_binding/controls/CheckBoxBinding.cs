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

    private string _textExpression;
    private string _tooltipExpression;
    private IUiControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IUiControlBinding binding) _binding = binding;
        else return;

        _textExpression = Text;
        _tooltipExpression = TooltipText;

        RegexTools.ExtractMatchingValues(Text, BindingTools.BindingRegex)
            .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) => UpdateTextAndToolTip()));
        RegexTools.ExtractMatchingValues(TooltipText, BindingTools.BindingRegex)
            .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) => UpdateTextAndToolTip()));

        UpdateTextAndToolTip();

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

    public void UpdateTextAndToolTip()
    {
        Text = BindingTools.BindReplacedMatchingValues(_textExpression, _binding);
        TooltipText = BindingTools.BindReplacedMatchingValues(_tooltipExpression, _binding);
    }
}
