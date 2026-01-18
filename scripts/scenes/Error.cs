using Godot;
using GodotLauncher.Scripts.Scenes.VersionsView;
using GodotLauncher.Scripts.Scenes.SettingsView;
using System;

namespace GodotLauncher.Scripts.Scenes;

public partial class Error : Control
{
    [Signal]
    public delegate void CloseEventHandler();

    private Label _errorLabel => GetNode<Label>("%ErrorLabel");

    public void SetErrorText(string errorText)
    {
        _errorLabel.Text = errorText;
    }

    private void ButtonOkDown()
    {
        EmitSignal(SignalName.Close);
    }
}
