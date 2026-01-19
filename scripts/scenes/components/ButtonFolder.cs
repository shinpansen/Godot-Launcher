using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Scenes.Components;

public partial class ButtonFolder : Button
{
    [Export]
    public string PathPropertyName { get; set; }

    [Export]
    public bool DirectoryPath { get; set; } = true;

    private void OnButtonOpenFolderDown()
    {
        if (GetOwner() is not IControlBinding binding) return;

        string dir = binding.GetPropertyValue(PathPropertyName).ToString();
        string directoryPath = DirectoryPath ? dir : System.IO.Path.GetDirectoryName(dir);
        if (!System.IO.Directory.Exists(directoryPath))
        {
            ErrorTools.ShowError($"{TranslationServer.Translate("!dirnotfound")} {directoryPath}");
            return;
        }
        SystemTools.OpenFileExplorer(directoryPath);
    }
}
