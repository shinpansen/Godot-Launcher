using Godot;
using System;
using System.Collections;
using System.Linq;

namespace GodotLauncher.Scripts.UiBinding.Controls;

public partial class BoxContainerBinding : BoxContainer
{
    [Signal]
    public delegate void DeleteItemEventHandler(Node node);

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
        GetChildren().ToList().ForEach(RemoveChild);

        if (items is IEnumerable enumerable)
        {
            foreach (var model in enumerable)
            {
                var instance = ItemScene.Instantiate();
                if (instance is IUiControlItem controlItem)
                    controlItem.Init(model);

                AddChild(instance);
            }
        }
    }
}
