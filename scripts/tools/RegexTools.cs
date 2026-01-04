using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class RegexTools
{
    public static string ReplaceMatchingValues(string expression, string regex, Func<string, string> replaceFunc)
    {
        string result = Regex.Replace(
            expression,
            regex,
            match =>
            {
                string bindingName = match.Groups[1].Value;
                return replaceFunc.Invoke(bindingName);
            });
        return result;
    }

    public static List<string> ExtractMatchingValues(string expression, string regex)
    {
        return Regex.Matches(expression, regex)
            .Cast<Match>()
            .Select(m => m.Groups[1].Value)
            .Distinct()
            .ToList();
    }
}
