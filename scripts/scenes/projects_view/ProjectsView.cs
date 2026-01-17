using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes.Components;
using GodotLauncher.Scripts.Scenes.VersionsView;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;

namespace GodotLauncher.Scripts.Scenes.ProjectsView;

public partial class ProjectsView : DataSourceBinding<ProjectsConfig>
{
    private Node _projectsVBoxContainer => GetNode("%ProjectsVBoxContainer");
    private TextEdit _textEditSearch => GetNode<TextEdit>("%TextEditSearch");
    private ButtonSort _buttonSortByLastEdit => GetNode<ButtonSort>("%ButtonSortByLastEdit");
    private ButtonSort _buttonSortByName => GetNode<ButtonSort>("%ButtonSortByName");

    public override void _Ready()
    {
        _buttonSortByLastEdit.ButtonPressed = BindingContext.SortType == Enums.ProjectSortType.LastEdit;
        _buttonSortByLastEdit.SortOrder = BindingContext.SortOrder;
        _buttonSortByName.ButtonPressed = BindingContext.SortType == Enums.ProjectSortType.ProjetctName;
        _buttonSortByName.SortOrder = BindingContext.SortOrder;
    }

    protected override ProjectsConfig LoadDataSource()
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        return LoadProjectsConfig(settings.ScanWhenLauncherStart);
    }

    public ProjectsConfig Refresh()
    {
        ProjectsConfig config = LoadProjectsConfig(true);
        SetPropertyValue(pc => pc.Projects, config.Projects);
        return config;
    }

    public void RefreshProjectsVersions(List<EngineVersion> versions)
    {
        ProjectsConfig config = LoadProjectsConfig(false);
        config.Projects = UserDataScanner.MatchAvailableVersions(config.Projects, versions);
        SetPropertyValue(pc => pc.Projects, config.Projects);
    }

    private ProjectsConfig LoadProjectsConfig(bool forceScan)
    {
        ProjectsConfig config = UserDataLoader.LoadUserProjects();
        if (forceScan)
        {
            var projectsScanned = UserDataScanner.ScanUserProjects();
            config = UserDataLoader.MergeUserProjectsConfig(config, projectsScanned);
        }

        VersionsConfig versionsConfig = UserDataLoader.LoadUserVersions();
        config.Projects = UserDataScanner.MatchAvailableVersions(config.Projects, versionsConfig.Versions);
        return config;
    }

    private void TextEditSearchChanged()
    {
        string text = _textEditSearch.Text;

        IEnumerable<ProjectItemView> children = _projectsVBoxContainer.GetChildren().OfType<ProjectItemView>();
        foreach (var p in children)
        {
            if(string.IsNullOrEmpty(text))
            {
                p.Visible = true;
                continue;
            }

            p.Visible = p.BindingContext.Name.Contains(text, StringComparison.CurrentCultureIgnoreCase);
        }
    }

    private void SortChanged(Node node)
    {
        if (node is not ButtonSort bs) return;

        BindingContext.SortType = (Enums.ProjectSortType)bs.ButtonIndex;
        BindingContext.SortOrder = bs.SortOrder;
        RefreshProjectsItemsOrder();
        SaveDataSource();
    }

    private void RefreshProjectsItemsOrder()
    {
        IEnumerable<ProjectItemView> children = _projectsVBoxContainer.GetChildren().OfType<ProjectItemView>();

        if (BindingContext.SortType == Enums.ProjectSortType.LastEdit)
        {
            var asc = BindingContext.SortOrder == Enums.SortOrder.Asc;
            children = (asc
                ? children.OrderBy(c => c.BindingContext.LastEdit ?? DateTime.MinValue)
                : children.OrderByDescending(c => c.BindingContext.LastEdit ?? DateTime.MinValue));
        }
        else if (BindingContext.SortType == Enums.ProjectSortType.ProjetctName)
        {
            var asc = BindingContext.SortOrder == Enums.SortOrder.Asc;
            children = (asc
                ? children.OrderBy(c => c.BindingContext.Name)
                : children.OrderByDescending(c => c.BindingContext.Name));
        }

        for (int i = 0; i < children.Count(); i++)
            _projectsVBoxContainer.MoveChild(children.ElementAt(i), i);
    }
}
