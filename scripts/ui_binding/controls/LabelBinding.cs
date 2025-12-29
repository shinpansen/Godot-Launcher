using Godot;
using GodotLauncher.Scripts.UiBinding;
using System;
using System.Text.RegularExpressions;

[GlobalClass]
public partial class LabelBinding : Label
{
    public override void _Ready()
    {
        UpdateUi();
    }

    public void UpdateUi()
    {
        if (BindTextValue(Text, out string textValue))
        {
            Text = VisibleCharacters > 3 && textValue.Length > VisibleCharacters ?
                textValue.Substring(0, VisibleCharacters - 3) + "..." :
                textValue;
        }
        if (BindTextValue(TooltipText, out string toolTipValue)) TooltipText = toolTipValue;
    }

    private bool BindTextValue(string expression, out string propertyValue)
    {
        propertyValue = string.Empty;
        var matchBinding = Regex.Match(expression, @"^\{([^}]+)\}$");
        if (matchBinding.Success && GetOwner() is IUiControlBinding binding)
        {
            string propertyName = matchBinding.Groups[1].Value;
            propertyValue = binding.GetPropertyValue(propertyName)?.ToString();
            return true;
        }
        return false;
    }
}
