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
    private const string BindingRegex = @"\{([^}]+)\}";
    private string _textExpression;
    private string _tooltipExpression;
    private IUiControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IUiControlBinding binding) _binding = binding;
        else return;

        _textExpression = Text;
        _tooltipExpression = TooltipText;

        RegexTools.ExtractMatchingValues(Text, BindingRegex)
            .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) => UpdateUi()));
        RegexTools.ExtractMatchingValues(TooltipText, BindingRegex)
            .ForEach(n => _binding.RegisterPropertyChangedEvent(n, (v) => UpdateUi()));

        UpdateUi();
    }

    public void UpdateUi()
    {
        string textValue = BindTextValue(_textExpression);
        Text = VisibleCharacters > 3 && textValue.Length > VisibleCharacters ?
            textValue.Substring(0, VisibleCharacters - 3) + "..." :
            textValue;

        TooltipText = BindTextValue(_tooltipExpression);
    }

    private string BindTextValue(string expression)
    {
        return RegexTools.ReplaceMatchingValues(expression, BindingRegex,
            (name) =>
            {
                return _binding.HasProperty(name) ?
                    _binding.GetPropertyValue<string>(name) :
                    "{" + name + "}";
            });
    }
}
