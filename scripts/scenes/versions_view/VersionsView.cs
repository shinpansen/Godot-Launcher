using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes;
using GodotLauncher.Scripts.UiBinding;
using GodotLauncher.Scripts.UserData;
using System;
using System.Linq;
using System.Collections.Generic;

namespace GodotLauncher.Scripts.Scenes.VersionsView;

public partial class VersionsView : UiControlDataSource<VersionsConfig>
{
    private HFlowContainer _versionsHFlowContainer => GetNode<HFlowContainer>("%VersionsHFlowContainer");
    private Button _buttonSortByVersion => GetNode<Button>("%ButtonSortByVersion");
    private Button _buttonSortByName => GetNode<Button>("%ButtonSortByName");
    private Texture2D _sortAscTexture = GD.Load<Texture2D>("res://assets/icons/sort-asc.svg");
    private Texture2D _sortDescTexture = GD.Load<Texture2D>("res://assets/icons/sort-desc.svg");

    public override void _Ready()
    {
        BindingContext.Versions.ForEach(e =>
        {
            PackedScene engineItemScene = GD.Load<PackedScene>("res://scenes/components/version_item.tscn");
            var item = engineItemScene.Instantiate<VersionItemView>();
            item.Init(e);
            _versionsHFlowContainer.AddChild(item);
        });

        UpdateSortButtonStateUI();
        UpdateVersionsItemsOrderUI();

        //using var file = FileAccess.Open("user://engines.json", FileAccess.ModeFlags.Write);

        //var item = new EngineItem("4.2.1", @"C:\Program Files (x86)\Godot\Godot_v4.2.1-stable_mono_win64\Godot_v4.2.1-stable_mono_win64.exe");
        //var item2 = new EngineItem("4.4.1", @"C:\Program Files (x86)\Godot\Godot_v4.4.1-stable_mono_win64\Godot_v4.4.1-stable_mono_win64.exe");
        //var items = new EnginesList()
        //{
        //    Engines = new System.Collections.Generic.List<EngineItem> { item, item2 },
        //};

        //file.StoreString(System.Text.Json.JsonSerializer.Serialize(items));
    }

    protected override VersionsConfig LoadDataSource()
    {
        VersionsConfig config = UserDataLoader.LoadUserVersions();
        var versionsScanned = UserDataScanner.ScanUserEngines();
        return UserDataLoader.MergeUserVersionsConfig(config, versionsScanned);
    }

    protected override void SaveDataSource() => UserDataLoader.SaveUserVersions(BindingContext);

    private void OnButtonSortByNameDown() => SortContext(Enums.EngineSortType.Name);

    private void OnButtonSortByVersionDown() => SortContext(Enums.EngineSortType.Version);

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
