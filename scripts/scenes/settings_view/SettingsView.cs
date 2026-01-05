using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UiBinding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;
using Godot;

namespace GodotLauncher.Scripts.Scenes.SettingsView;

public partial class SettingsView : UiControlDataSource<Settings>
{
    protected override Settings LoadDataSource() => UserDataLoader.LoadUserSettings();
    protected override void SaveDataSource() => UserDataLoader.SaveUserSettings(BindingContext);

    private void Test()
    {
        var paths = BindingContext.CustomInstallDirectories;
        paths.Add(new FileSystemPath("gloubi"));
        SetPropertyValue(s => s.CustomInstallDirectories, paths);
    }

    private void DeleteItem(Node node)
    {
        if (node is UiControlItem<FileSystemPath> item)
        {
            var paths = BindingContext.CustomInstallDirectories;
            paths.Remove(item.BindingContext);
            SetPropertyValue(s => s.CustomInstallDirectories, paths);
        }
    }
}
