using GodotLauncher.Scripts.Binding.Interfaces;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class StringTools
{
    public static string FirstLetterUpper(string text)
    {
        if (string.IsNullOrEmpty(text)) return text;
        return text.Substring(0,1).ToUpper() + text.Substring(1);
    }
}
