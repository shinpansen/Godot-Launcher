using Godot;
using System;
using System.Collections;

namespace GodotLauncher.Scripts.UiBinding.Controls;

public partial class BoxContainerBinding : BoxContainer
{
    [Export]
    public string BindingListPropertyName { get; set; }

    [Export]
    public PackedScene ItemScene { get; set; }

    private IUiControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IUiControlBinding binding) _binding = binding;
        else return;

        _binding.RegisterPropertyChangedEvent(BindingListPropertyName, UpdateItems);
        var items = _binding.GetPropertyValue(BindingListPropertyName);
        UpdateItems(items);
    }

    private void UpdateItems(object items)
    {
        if (items is IEnumerable enumerable)
        {
            GD.Print("items is IEnumerable");
            foreach (var item in enumerable)
            {
                if (item is UiModel model)
                {
                    GD.Print("item is UiModel");
                    var instance = ItemScene.Instantiate();
                    if (instance is IUiControlItem controlItem)
                    {
                        controlItem.Init(model);
                        AddChild(instance);
                    }
                }
            }
        }
    }
}
