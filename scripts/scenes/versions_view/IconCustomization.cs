using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Models;
using System;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class IconCustomization : DataSourceBinding<CustomIcon>
{
    [Signal]
    public delegate void SaveEventHandler(Node node);

    [Signal]
    public delegate void CancelEventHandler();

    public void UpdateSettings(CustomIcon icon)
    {
        SetPropertyValue(c => c.Background, icon.Background);
        SetPropertyValue(c => c.GrayScale, icon.GrayScale);
        SetPropertyValue(c => c.Color, icon.Color);
    }

    protected override CustomIcon LoadDataSource()
    {
        return new CustomIcon();
    }

    private void OnButtonResetDown()
    {
        UpdateSettings(new CustomIcon());
    }

    private void OnButtonSaveDown()
    {
        EmitSignal(SignalName.Save, this);
    }

    private void OnButtonCancelDown()
    {
        EmitSignal(SignalName.Cancel);
    }
}
