using Godot;
using GodotLauncher.Scripts.UiBinding;
using System;
using System.Text.RegularExpressions;

[GlobalClass]
public partial class LabelBinding : Label
{
    private string _textExpresion;
    private string _tooltipExpresion;
    private IUiControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IUiControlBinding binding) _binding = binding;
        else return;

        _binding.RegisterControl(GetInstanceId());
        _textExpresion = Text;
        _tooltipExpresion = TooltipText;
        UpdateUi();
    }

    public override void _Process(double delta)
    {
        if (_binding is null) return;
        else if (_binding.HasChanged(GetInstanceId())) UpdateUi();
    }

    public void UpdateUi()
    {
        if (BindTextValue(_textExpresion, out string textValue))
        {
            Text = VisibleCharacters > 3 && textValue.Length > VisibleCharacters ?
                textValue.Substring(0, VisibleCharacters - 3) + "..." :
                textValue;
        }
        if (BindTextValue(_tooltipExpresion, out string toolTipValue)) TooltipText = toolTipValue;
    }

    private bool BindTextValue(string expression, out string propertyValue)
    {
        propertyValue = string.Empty;
        var matchBinding = Regex.Match(expression, @"^\{([^}]+)\}$");
        if (matchBinding.Success)
        {
            string propertyName = matchBinding.Groups[1].Value;
            propertyValue = _binding.GetPropertyValue<string>(propertyName);
            return true;
        }
        return false;
    }
}
