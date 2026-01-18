using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Extensions;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace GodotLauncher.Scripts.Scenes.ProjectsView;

public partial class LaunchProjectSettings : Node
{
    [Signal]
    public delegate void SaveEventHandler(Node node);

    [Signal]
    public delegate void CancelEventHandler();

    public EngineVersion SelectedVersion => 
        _optionsVersions.Selected == 0 ? 
        null : 
        _availableVersions[_optionsVersions.Selected - 1];

    public string Args => _textEditArgs.Text;

    private OptionButton _optionsVersions => GetNode<OptionButton>("%OptionsVersions");
    private TextEdit _textEditArgs => GetNode<TextEdit>("%TextEditArgs");
    private List<EngineVersion> _availableVersions;

    public void UpdateSettings(
        EngineVersion currentVersion, 
        EngineVersion optimalVersion,
        List<EngineVersion> availableVersions,
        string args)
    {
        _textEditArgs.Text = args;
        _availableVersions = availableVersions;

        _optionsVersions.Clear();
        int selectedIndex = 0;

        //Auto version = optimal
        string label = optimalVersion.FormatedName;
        _optionsVersions.AddItem($"Auto: {label}");

        for (int i = 0; i < availableVersions.Count; i++)
        {
            label = availableVersions[i].FormatedName;
            if (currentVersion is not null && label == currentVersion.FormatedName) 
                selectedIndex = i + 1;
            _optionsVersions.AddItem(label, i + 1);
        }
        _optionsVersions.Select(selectedIndex);
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
