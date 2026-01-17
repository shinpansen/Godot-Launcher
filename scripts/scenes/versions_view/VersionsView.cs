using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes;
using GodotLauncher.Scripts.Scenes.Components;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using static Godot.VisualShaderNode;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionsView : DataSourceBinding<VersionsConfig>
{
    private Node _versionsHFlowContainer => GetNode("%VersionsHFlowContainer");
    private ButtonSort _buttonSortByVersion => GetNode<ButtonSort>("%ButtonSortByVersion");
    private ButtonSort _buttonSortByFileName => GetNode<ButtonSort>("%ButtonSortByFileName");
    private Texture2D _sortAscTexture = GD.Load<Texture2D>("res://assets/icons/sort-asc.svg");
    private Texture2D _sortDescTexture = GD.Load<Texture2D>("res://assets/icons/sort-desc.svg");

    public override void _Ready()
    {
        _buttonSortByVersion.ButtonPressed = BindingContext.SortType == Enums.EngineSortType.Version;
        _buttonSortByVersion.SortOrder = BindingContext.SortOrder;
        _buttonSortByFileName.ButtonPressed = BindingContext.SortType == Enums.EngineSortType.FileName;
        _buttonSortByFileName.SortOrder = BindingContext.SortOrder;

        RefreshVersionsItemsOrder();
    }

    public VersionsConfig Refresh()
    {
        VersionsConfig config = LoadVersionConfig(true);
        SetPropertyValue(vc => vc.Versions, config.Versions);
        RefreshVersionsItemsOrder();
        return config;
    }

    protected override VersionsConfig LoadDataSource()
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        return LoadVersionConfig(settings.ScanWhenLauncherStart);
    }

    private VersionsConfig LoadVersionConfig(bool forceScan)
    {
        VersionsConfig config = UserDataLoader.LoadUserVersions();
        if (forceScan)
        {
            var versionsScanned = UserDataScanner.ScanUserEngines();
            config = UserDataLoader.MergeUserVersionsConfig(config, versionsScanned);
        }
        return config;
    }

    private void SortChanged(Node node)
    {
        if (node is not ButtonSort bs) return;

        BindingContext.SortType = (Enums.EngineSortType)bs.ButtonIndex;
        BindingContext.SortOrder = bs.SortOrder;
        RefreshVersionsItemsOrder();
        SaveDataSource();
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
                .ThenByDescending(c => GodotVersionType.Parse(c.BindingContext.Type).Number)
                .ThenBy(c => c.BindingContext.Mono == true ? 2 : 1);
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
