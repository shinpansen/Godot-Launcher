using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace GodotLauncher.Scripts.Tools;

public static class BindingTools
{
    public const string BindingRegex = @"\{([^}]+)\}";
    public const string TernaryOpRegex = @"(?<condition>[^?]+)\?\s*(?<true>[^:]+)\s*:\s*(?<false>.+)";

    public static List<string> ExtractBindingValues(string expression)
    {
        List<string> bindingValues = [];
        List<string> expressions = RegexTools.ExtractMatchingValues(expression, BindingRegex);
        foreach(var expr in expressions)
        {
            if(IsTernaryExpression(expr, out Match match))
            {
                string condition = match.Groups["condition"].Value.Trim();
                bindingValues.Add(condition);
            }
            else
                bindingValues.Add(expr);
        }
        return bindingValues;
    }

    public static string BindReplacedMatchingValues(string expression, IControlBinding binding)
    {
        return RegexTools.ReplaceMatchingValues(expression, BindingRegex,
            (match) =>
            {
                if (IsTernaryExpression(match, out _))
                    return ReplaceTernaryExpression(match, binding);
                else if (binding.HasProperty(match))
                    return binding.GetPropertyValue<string>(match);
                else
                    return "{" + match + "}";
            });
    }

    public static bool IsTernaryExpression(string expression, out Match match)
    {
        var regex = new Regex(TernaryOpRegex);
        match = regex.Match(expression);
        return match.Success;
    }

    public static string ReplaceTernaryExpression(string expression, IControlBinding binding)
    {
        if (IsTernaryExpression(expression, out Match match))
        {
            string condition = match.Groups["condition"].Value.Trim();
            string trueValue = match.Groups["true"].Value.Trim();
            string falseValue = match.Groups["false"].Value.Trim();

            bool conditionValue = binding.GetPropertyValue<bool>(condition);
            string propertyOrValue = conditionValue ? trueValue : falseValue;

            if (!binding.HasProperty(condition))
                return expression;
            if (binding.GetPropertyType(condition) != typeof(bool))
                return expression;

            if (binding.HasProperty(propertyOrValue))
                return binding.GetPropertyValue<string>(propertyOrValue);
            else if (int.TryParse(propertyOrValue, out _) || float.TryParse(propertyOrValue, out _))
                return propertyOrValue;
            else if (propertyOrValue.StartsWith("\"") && propertyOrValue.EndsWith("\""))
                return propertyOrValue.Replace("\"", "");
            else
                return expression;
        }
        else
            return expression;
    }
}
