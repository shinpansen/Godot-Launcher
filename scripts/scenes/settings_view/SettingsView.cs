using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UiBinding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Linq.Expressions;

namespace GodotLauncher.Scripts.Scenes.SettingsView;

public partial class SettingsView : UiControlDataSource<Settings>
{
    protected override Settings LoadBindingContext() => UserDataLoader.LoadUserSettings();

    public override void SetPropertyValue(string propertyName, object propertyValue)
    {
        base.SetPropertyValue(propertyName, propertyValue);
        SaveConfig();
    }

    protected override void SetPropertyValue<TValue>(Expression<Func<Settings, TValue>> member, object propertyValue)
    {
        base.SetPropertyValue(member, propertyValue);
        SaveConfig();
    }

    private void SaveConfig()
    {
        UserDataLoader.SaveUserSettings(BindingContext);
    }
}
