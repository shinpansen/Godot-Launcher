using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Binding.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Scenes.SettingsView;

public partial class PathItemView : ItemBinding<FileSystemPath>
{
    private void OnButtonDeleteDown()
    {
        var parent = GetParent<BoxContainerBinding>();
        parent?.EmitSignal(
            BoxContainerBinding.SignalName.DeleteItem,
            this
        );
    }

    private void OnDirSelected(string dirPath)
    {
        SetPropertyValue(s => s.FullName, dirPath);
    }
}
