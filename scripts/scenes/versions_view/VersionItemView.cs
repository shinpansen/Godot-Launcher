using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using GodotLauncher.Scripts.Binding.Controls;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionItemView : ItemBinding<EngineVersion>
{
    private TextureRect _icon => GetNode<TextureRect>("%Icon");
    private Window _windowEdit => GetNode<Window>("%WindowEdit");
    private IconCustomization _iconCustomization => GetNode<IconCustomization>("%IconCustomization");
    private MenuButton _menuButton => GetNode<MenuButton>("%MenuButton");
    private VersionsView _versionsView;


    public override void _Ready()
    {
        _menuButton.GetPopup().IdPressed += OnMenuButtonIdPressed;
        if (DataSource is VersionsView v) _versionsView = v;
    }

    private void OnButtonLaunchDown()
    {
        
        if(!System.IO.File.Exists(BindingContext.Path))
        {
            DialogTools.ShowError($"{TranslationServer.Translate("!exenotfound")} {BindingContext.Path}");
            return;
        }

        SystemTools.OpenPath(BindingContext.Path); 
        Settings settings = UserDataLoader.LoadUserSettings();
        if (settings.CloseLauncherWhenStartingGodot) GetTree().Quit();
    }

    private void OnMenuButtonIdPressed(long id)
    {
        switch(id)
        {
            case 0:
                _windowEdit.Show();
                _iconCustomization.UpdateSettings(BindingContext.CustomIcon);
                break;
            case 1:
                Visible = false;
                _versionsView?.SettingsView?.AddExcludedFile(BindingContext.Path);
                break;
            case 2:
                string message = TranslationServer.Translate("!sureuninstall");
                DialogTools.ShowQuestion(TranslationServer.Translate("!sure"),
                    string.Format(message, BindingContext.FileName), () => UninstallVersion());
                break;
        }
    }

    private void UninstallVersion()
    {
        string path = System.IO.Path.GetDirectoryName(BindingContext.Path);
        try
        {
            System.IO.Directory.Delete(path, true);

            var parent = GetParent<BoxContainerBinding>();
            parent?.EmitSignal(
                BoxContainerBinding.SignalName.DeleteItem,
                this
            );
        }
        catch (Exception ex)
        {
            DialogTools.ShowError(ex.Message);
        }
    }

    private void HideWindowEdit()
    {
        _windowEdit.Hide();
    }

    private void SaveCustomIcon(Node iconCustomization)
    {
        if (iconCustomization is IconCustomization ic)
        {
            SetPropertyValue(v => v.CustomIcon, new CustomIcon()
            {
                Background = ic.BindingContext.Background,
                GrayScale = ic.BindingContext.GrayScale,
                HexColor = ic.BindingContext.HexColor,
                Color = ic.BindingContext.Color
            });
            GetNode<GenericBinding>("%Icon").Refresh();
            HideWindowEdit();
        }
        else
            GD.PrintErr("Can't save custom icon. Invalid node type");
    }
}