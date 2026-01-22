using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes;
using GodotLauncher.Scripts.Scenes.Components;
using GodotLauncher.Scripts.Scenes.ProjectsView;
using GodotLauncher.Scripts.Tools;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.VisualShaderNode;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionsView : DataSourceBinding<VersionsConfig>
{
    [Export]
    public SettingsView.SettingsView SettingsView { get; set; }

    private Node _versionsHFlowContainer => GetNode("%VersionsHFlowContainer");
    private TextEdit _textEditSearch => GetNode<TextEdit>("%TextEditSearch");
    private ButtonSort _buttonSortByVersion => GetNode<ButtonSort>("%ButtonSortByVersion");
    private ButtonSort _buttonSortByFileName => GetNode<ButtonSort>("%ButtonSortByFileName");
    private Texture2D _sortAscTexture = GD.Load<Texture2D>("res://assets/icons/sort-asc.svg");
    private Texture2D _sortDescTexture = GD.Load<Texture2D>("res://assets/icons/sort-desc.svg");

    private List<EngineVersion> _versionsTemp;

    public override void _Ready()
    {
        _buttonSortByVersion.ButtonPressed = BindingContext.SortType == Enums.EngineSortType.Version;
        _buttonSortByVersion.SortOrder = BindingContext.SortOrder;
        _buttonSortByFileName.ButtonPressed = BindingContext.SortType == Enums.EngineSortType.FileName;
        _buttonSortByFileName.SortOrder = BindingContext.SortOrder;

        RefreshVersionsItemsOrder();
    }

    public VersionsConfig Refresh(out string errors)
    {
        VersionsConfig config = LoadVersionConfig(true, out errors);
        _versionsTemp = config.Versions;
        //SetPropertyValue(vc => vc.Versions, config.Versions);
        CallDeferred(nameof(UpdateVersionsWithTempProperty));
        CallDeferred(nameof(RefreshVersionsItemsOrder));
        return config;
    }

    protected override VersionsConfig LoadDataSource()
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        var config = LoadVersionConfig(settings.ScanWhenLauncherStart, out string errors);

        if (!string.IsNullOrEmpty(errors))
            DialogTools.ShowError(errors);
        return config;
    }

    private VersionsConfig LoadVersionConfig(bool forceScan, out string errors)
    {
        errors = string.Empty;
        VersionsConfig config = UserDataLoader.LoadUserVersions();
        if (forceScan)
        {
            var versionsScanned = UserDataScanner.ScanUserEngines(out errors);
            config = UserDataLoader.MergeUserVersionsConfig(config, versionsScanned);
        }
        return config;
    }

    private void TextEditSearchChanged()
    {
        string text = _textEditSearch.Text;

        IEnumerable<VersionItemView> children = _versionsHFlowContainer.GetChildren().OfType<VersionItemView>();
        foreach (var p in children)
        {
            if (string.IsNullOrEmpty(text))
            {
                p.Visible = true;
                continue;
            }

            p.Visible = p.BindingContext.Version.Contains(text, StringComparison.CurrentCultureIgnoreCase) ||
                p.BindingContext.Type.Contains(text, StringComparison.CurrentCultureIgnoreCase) ||
                p.BindingContext.FileName.Contains(text, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    private void SortChanged(Node node)
    {
        if (node is not ButtonSort bs) return;

        BindingContext.SortType = (Enums.EngineSortType)bs.ButtonIndex;
        BindingContext.SortOrder = bs.SortOrder;
        RefreshVersionsItemsOrder();
        SaveDataSource();
    }

    private void UpdateVersionsWithTempProperty()
    {
        SetPropertyValue(vc => vc.Versions, _versionsTemp);
        _versionsTemp?.Clear();
        _versionsTemp = null;
    }

    private void RefreshVersionsItemsOrder()
    {
        IEnumerable<VersionItemView> children = _versionsHFlowContainer.GetChildren().OfType<VersionItemView>();

        if (BindingContext.SortType == Enums.EngineSortType.Version)
        {
            var asc = BindingContext.SortOrder == Enums.SortOrder.Asc;
            children = (asc
                    ? children.OrderBy(c => c.BindingContext.Version)
                    : children.OrderByDescending(c => c.BindingContext.Version))
                .ThenByDescending(c => GodotVersionType.Parse(c.BindingContext.Type).Kind)
                .ThenByDescending(c => GodotVersionType.Parse(c.BindingContext.Type).Number ?? int.MinValue)
                .ThenBy(c => c.BindingContext.Mono == true ? 2 : 1)
                .ThenByDescending(c => c.BindingContext.ExeBitness);
        }
        else if (BindingContext.SortType == Enums.EngineSortType.FileName)
        {
            var asc = BindingContext.SortOrder == Enums.SortOrder.Asc;
            children = (asc
                    ? children.OrderBy(c => c.BindingContext.FileName)
                    : children.OrderByDescending(c => c.BindingContext.FileName))
                .ThenBy(c => c.BindingContext.Mono == true ? 2 : 1);
        }

        for (int i = 0; i < children.Count(); i++)
            _versionsHFlowContainer.MoveChild(children.ElementAt(i), i);
    }
}
