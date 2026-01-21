using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Binding.Controls;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Scenes.DownloadsView;

public partial class DownloadsView : Node
{
    [Signal]
    public delegate void CloseEventHandler(Node node);

    private OptionButton _optionsVersions => GetNode< OptionButton>("%OptionsVersions");
    private OptionButton _optionsTypes => GetNode<OptionButton>("%OptionsTypes");
    private CheckButton _checkButtonMono => GetNode<CheckButton>("%CheckButtonMono");

    private DownloadConfig _config = new();

    public void Load()
    {
        _config.AvailableReleases = GodotDownloader.DownloadReleasesInfo();

        int i = 0;
        _optionsVersions.Clear();
        _config.AvailableReleases.ForEach(v =>
        {
            _optionsVersions.GetPopup().AddItem(v.Name, i++);
        });

        _optionsVersions.Selected = 0;
        OnOptionsVersionsItemSelected(0);
    }

    private void OnOptionsVersionsItemSelected(long index)
    {
        Release selectedRelease = _config.AvailableReleases[(int)index];
        _config.AvailableTypes = selectedRelease.Releases.Select(r =>  r.Name).ToList();

        int i = 0;
        _optionsTypes.Clear();
        _config.AvailableTypes.ForEach(r =>
        {
            _optionsTypes.GetPopup().AddItem(r, i++);
        });
        _optionsTypes.Selected = 0;

        if (new Version(selectedRelease.Name).Major < 3)
        {
            _config.MonoAvailable = false;
            _checkButtonMono.ButtonPressed = false;
        }
        else
            _config.MonoAvailable = true;
        _checkButtonMono.Visible = _config.MonoAvailable;
    }

    private void OnButtonCloseDown()
    {
        EmitSignal(SignalName.Close);
    }
}
