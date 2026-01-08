using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;

namespace GodotLauncher.Scripts.Binding.Controls;

[GlobalClass]
public partial class LabelBinding : Label
{
    private string _textExpression;
    private string _tooltipExpression;
    private IControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IControlBinding binding) _binding = binding;
        else return;

        _textExpression = Text;
        _tooltipExpression = TooltipText;

        RegexTools.ExtractMatchingValues(Text, BindingTools.BindingRegex)
            .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) => UpdateTextAndToolTip()));
        RegexTools.ExtractMatchingValues(TooltipText, BindingTools.BindingRegex)
            .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) => UpdateTextAndToolTip()));

        UpdateTextAndToolTip();
    }

    public void UpdateTextAndToolTip()
    {
        string textValue = BindingTools.BindReplacedMatchingValues(_textExpression, _binding);
        Text = VisibleCharacters > 3 && textValue.Length > VisibleCharacters ?
            textValue.Substring(0, VisibleCharacters - 3) + "..." :
            textValue;

        TooltipText = BindingTools.BindReplacedMatchingValues(_tooltipExpression, _binding);
    }
}
