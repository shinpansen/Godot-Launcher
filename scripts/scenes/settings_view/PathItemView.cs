using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UiBinding;
using GodotLauncher.Scripts.UiBinding.Controls;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Scenes.SettingsView;

public partial class PathItemView : UiControlItem<FileSystemPath>
{
    private void Delete()
    {
        var parent = GetParent<BoxContainerBinding>();
        parent?.EmitSignal(
            BoxContainerBinding.SignalName.DeleteItem,
            this
        );
    }
}
