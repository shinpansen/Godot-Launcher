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
public partial class LabelBinding : Label
{
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
