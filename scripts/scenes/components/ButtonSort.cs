using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Scenes.Components;

[GlobalClass]
public partial class ButtonSort : Button
{
    [Signal]
    public delegate void SortChangedEventHandler(Node node);

    [Export]
    public int ButtonIndex { get; set; }

    public Enums.SortOrder SortOrder
    {
        get => _sortOrder;
        set
        {
            _sortOrder = value;
            UpdateButtonIcon();
        }
    }

    private Texture2D _sortAscTexture = GD.Load<Texture2D>("res://assets/icons/sort-asc.svg");
    private Texture2D _sortDescTexture = GD.Load<Texture2D>("res://assets/icons/sort-desc.svg");

    private Enums.SortOrder _sortOrder;

    public override void _Ready()
    {
        this.ToggleMode = true;
        this.ButtonDown += SortViewItem;
        UpdateButtonIcon();
    }

    private void UpdateButtonIcon()
    {
        switch (_sortOrder)
        {
            case Enums.SortOrder.Asc:
                this.Icon = _sortAscTexture;
                break;
            case Enums.SortOrder.Desc:
                this.Icon = _sortDescTexture;
                break;
        }
    }

    private void SortViewItem()
    {
        if (this.ButtonPressed)
        {
            _sortOrder =
                _sortOrder == Enums.SortOrder.Asc ?
                Enums.SortOrder.Desc :
                Enums.SortOrder.Asc;
        }

        UpdateButtonIcon();
        EmitSignal(SignalName.SortChanged, this);
    }
}
