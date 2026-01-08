using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using System;
using System.Collections;
using System.Linq;

namespace GodotLauncher.Scripts.Binding.Controls;

public partial class BoxContainerBinding : Container
{
    [Signal]
    public delegate void DeleteItemEventHandler(Node node);

    [Export]
    public string BindingListPropertyName { get; set; }

    [Export]
    public PackedScene ItemScene { get; set; }

    private IControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IControlBinding binding) _binding = binding;
        else return;

        _binding.RegisterPropertyChangedEvent(BindingListPropertyName, UpdateItems);
        var items = _binding.GetPropertyValue(BindingListPropertyName);
        UpdateItems(items);
    }

    private void UpdateItems(object items)
    {
        GetChildren().ToList().ForEach(RemoveChild);

        var dataSource = _binding is IDataSourceBinding d ? d : null;
        if (items is IEnumerable enumerable)
        {
            foreach (var model in enumerable)
            {
                var instance = ItemScene.Instantiate();
                if (instance is IItemBinding controlItem)
                    controlItem.Init(model, dataSource);

                AddChild(instance);
            }
        }
    }
}
