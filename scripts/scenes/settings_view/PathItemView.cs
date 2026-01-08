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
    private FileDialog _fileDialogChangePath => GetNode<FileDialog>("%FileDialogChangePath");

    private void OnButtonEditDown()
    {
        _fileDialogChangePath.RootSubfolder = BindingContext.FullName;
        _fileDialogChangePath.Show();
    }

    private void OnDirSelected(string dirPath)
    {
        SetPropertyValue(s => s.FullName, dirPath);
    }

    private void OnButtonDeleteDown()
    {
        var parent = GetParent<BoxContainerBinding>();
        parent?.EmitSignal(
            BoxContainerBinding.SignalName.DeleteItem,
            this
        );
    }
}
