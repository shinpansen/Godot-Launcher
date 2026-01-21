using Godot;
using GodotLauncher.Scripts.Binding.Interfaces;
using GodotLauncher.Scripts.Extensions;
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

    [Export]
    public uint BottomMargin { get; set; } = 0;

    private IControlBinding _binding;

    public override void _Ready()
    {
        if (GetOwner() is IControlBinding binding) _binding = binding;
        else return;

        _binding.RegisterPropertyChangedEvent(BindingListPropertyName, UpdateItems);
        Refresh();
    }

    public void Refresh()
    {
        var items = _binding.GetPropertyValue(BindingListPropertyName);
        UpdateItems(items);
    }

    private void UpdateItems(object items)
    {
        this.Clear();

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

            if(BottomMargin > 0)
            {
                HSeparator hSeparator = new HSeparator();
                hSeparator.AddThemeStyleboxOverride("separator", new StyleBoxEmpty());
                hSeparator.AddThemeConstantOverride("separation", (int)BottomMargin);
                AddChild(hSeparator);
            }
        }
    }
}
