using Godot;
using GodotLauncher.Scripts.Enums;
using GodotLauncher.Scripts.Scenes.SettingsView;
using GodotLauncher.Scripts.Scenes.VersionsView;
using System;

namespace GodotLauncher.Scripts.Scenes;

public partial class Dialog : Control
{
    [Signal]
    public delegate void CloseEventHandler();

    private Label _textLabel => GetNode<Label>("%TextLabel");
    private Button _buttonValid => GetNode<Button>("%ButtonValid");
    private Button _buttonOkNo => GetNode<Button>("%ButtonOkNo");

    private Action _validCallBack;

    public void InitDialog(string text, DialogMode dialogMode = DialogMode.Message, Action ValidCallBack = null)
    {
        _textLabel.Text = text;
        _buttonValid.Visible = dialogMode == DialogMode.Question;
        _buttonOkNo.Text = dialogMode == DialogMode.Question ?
            TranslationServer.Translate("!no") :
            "OK";
        _validCallBack = ValidCallBack;
    }

    private void ButtonValidDown()
    {
        EmitSignal(SignalName.Close);
        _validCallBack?.Invoke();
    }

    private void ButtonOkDown()
    {
        EmitSignal(SignalName.Close);
    }
}
