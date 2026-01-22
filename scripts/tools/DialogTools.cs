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

public static class DialogTools
{
    public static Window ErrorWindow { get; set; }

    public static void ShowMessage(string title, string errorText)
    {
        ErrorWindow.Title = title;
        if (ErrorWindow?.GetChild(0) is GodotLauncher.Scripts.Scenes.Error e)
        {
            ErrorWindow.Show();
            e.SetErrorText(errorText);
        }
    }

    public static void ShowError(string errorText)
    {
        ErrorWindow.Title = TranslationServer.Translate("!error");
        if (ErrorWindow?.GetChild(0) is GodotLauncher.Scripts.Scenes.Error e)
        {
            ErrorWindow.Show();
            e.SetErrorText(errorText);
        }
    }
}
