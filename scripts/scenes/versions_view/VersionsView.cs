using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionsView : DataSourceBinding<VersionsConfig>
{
    private Node _versionsHFlowContainer => GetNode("%VersionsHFlowContainer");
    private Button _buttonSortByVersion => GetNode<Button>("%ButtonSortByVersion");
    private Button _buttonSortByName => GetNode<Button>("%ButtonSortByName");
    private Texture2D _sortAscTexture = GD.Load<Texture2D>("res://assets/icons/sort-asc.svg");
    private Texture2D _sortDescTexture = GD.Load<Texture2D>("res://assets/icons/sort-desc.svg");

    public override void _Ready()
    {
        UpdateSortButtonStateUI();
        UpdateVersionsItemsOrderUI();
    }

    public int Refresh()
    {
        VersionsConfig config = LoadVersionConfig(true);
        SetPropertyValue(vc => vc.Versions, config.Versions);
        UpdateVersionsItemsOrderUI();
        return config.Versions.Count;
    }

    protected override VersionsConfig LoadDataSource()
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        return LoadVersionConfig(settings.ScanWhenLauncherStart);
    }

    private void OnButtonSortByNameDown() => SortContext(Enums.EngineSortType.Name);

    private void OnButtonSortByVersionDown() => SortContext(Enums.EngineSortType.Version);

    private VersionsConfig LoadVersionConfig(bool forceScan)
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        VersionsConfig config = UserDataLoader.LoadUserVersions();
        if (forceScan)
        {
            var versionsScanned = UserDataScanner.ScanUserEngines();
            config = UserDataLoader.MergeUserVersionsConfig(config, versionsScanned);
        }
        return config;
    }

    private void UpdateSortButtonStateUI()
    {
        _buttonSortByName.Icon = null;
        _buttonSortByVersion.Icon = null;

        switch (BindingContext.SortType)
        {
            case Enums.EngineSortType.Name:
                _buttonSortByName.ButtonPressed = true;
                UpdateButtonIcon(_buttonSortByName);
                break;
            case Enums.EngineSortType.Version:
            default:
                _buttonSortByVersion.ButtonPressed = true;
                UpdateButtonIcon(_buttonSortByVersion);
                break;
        }
    }

    private void UpdateButtonIcon(Button button)
    {
        switch (BindingContext.SortOrder)
        {
            case Enums.SortOrder.Asc:
                button.Icon = _sortAscTexture;
                break;
            case Enums.SortOrder.Desc:
                button.Icon = _sortDescTexture;
                break;
        }
    }

    private void UpdateVersionsItemsOrderUI()
    {
        IEnumerable<VersionItemView> children = _versionsHFlowContainer.GetChildren().OfType<VersionItemView>();

        if (BindingContext.SortType == Enums.EngineSortType.Version && BindingContext.SortOrder == Enums.SortOrder.Asc)
            children = children.OrderBy(c => c.BindingContext.Version);
        else if (BindingContext.SortType == Enums.EngineSortType.Version && BindingContext.SortOrder == Enums.SortOrder.Desc)
            children = children.OrderByDescending(c => c.BindingContext.Version);
        else if (BindingContext.SortType == Enums.EngineSortType.Name && BindingContext.SortOrder == Enums.SortOrder.Asc)
            children = children.OrderBy(c => c.BindingContext.FileName);
        else if (BindingContext.SortType == Enums.EngineSortType.Name && BindingContext.SortOrder == Enums.SortOrder.Desc)
            children = children.OrderByDescending(c => c.BindingContext.FileName);

        for (int i = 0; i < children.Count(); i++)
            _versionsHFlowContainer.MoveChild(children.ElementAt(i), i);
    }

    private void SortContext(Enums.EngineSortType sortType)
    {
        if (sortType == BindingContext.SortType)
        {
            BindingContext.SortOrder = 
                BindingContext.SortOrder == Enums.SortOrder.Asc ?
                Enums.SortOrder.Desc : 
                Enums.SortOrder.Asc;
        }
        BindingContext.SortType = sortType;

        UpdateSortButtonStateUI();
        UpdateVersionsItemsOrderUI();
        SaveDataSource();
    }
}
