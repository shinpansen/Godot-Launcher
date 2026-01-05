using GodotLauncher.Scripts.UiBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class BindingTools
{
    public const string BindingRegex = @"\{([^}]+)\}";

    public static string BindReplacedMatchingValues(string expression, IUiControlBinding binding)
    {
        return RegexTools.ReplaceMatchingValues(expression, BindingRegex,
            (name) =>
            {
                return binding.HasProperty(name) ?
                    binding.GetPropertyValue<string>(name) :
                    "{" + name + "}";
            });
    }
}
