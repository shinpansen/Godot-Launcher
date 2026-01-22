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
    public static Window DialogWindow { get; set; }

    public static void ShowMessage(string title, string message)
    {
        DialogWindow.Title = title;
        if (DialogWindow?.GetChild(0) is GodotLauncher.Scripts.Scenes.Dialog e)
        {
            DialogWindow.Show();
            e.InitDialog(message);
        }
    }

    public static void ShowError(string errorText)
    {
        DialogWindow.Title = TranslationServer.Translate("!error");
        if (DialogWindow?.GetChild(0) is GodotLauncher.Scripts.Scenes.Dialog e)
        {
            DialogWindow.Show();
            e.InitDialog(errorText);
        }
    }

    public static void ShowQuestion(string title, string message, Action validCallBack)
    {
        DialogWindow.Title = title;
        if (DialogWindow?.GetChild(0) is GodotLauncher.Scripts.Scenes.Dialog e)
        {
            DialogWindow.Show();
            e.InitDialog(message, Enums.DialogMode.Question, validCallBack);
        }
    }
}
