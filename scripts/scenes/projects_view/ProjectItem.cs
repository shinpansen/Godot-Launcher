using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Extensions;
using GodotLauncher.Scripts.Models;
using System;

namespace GodotLauncher.Scripts.Scenes.ProjectsView;

public partial class ProjectItem : ItemBinding<Project>
{
    private TextureRect _projectIcon => GetNode<TextureRect>("%ProjectIcon");

    public override void _Ready()
    {
        UpdateIcon();
    }

    private void UpdateIcon()
    {
        if (string.IsNullOrEmpty(BindingContext.IconPath)) return;

        string iconPath = BindingContext.IconPath;
        if (iconPath.StartsWith(@"res://"))
            iconPath = @$"{BindingContext.Path}/{iconPath.Substring(6)}";
        _projectIcon.SetTextureFromPath(iconPath);
    }
}
