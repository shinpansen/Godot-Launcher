using Godot;
using GodotLauncher.Scripts.Binding;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;

namespace GodotLauncher.Scripts.Scenes.ProjectsView;

public partial class ProjectsView : DataSourceBinding<ProjectsConfig>
{
    protected override ProjectsConfig LoadDataSource()
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        return LoadProjectsConfig(settings.ScanWhenLauncherStart);
    }

    public int Refresh()
    {
        ProjectsConfig config = LoadProjectsConfig(true);
        SetPropertyValue(pc => pc.Projects, config.Projects);
        return config.Projects.Count;
        //UpdateVersionsItemsOrderUI();
    }

    private ProjectsConfig LoadProjectsConfig(bool forceScan)
    {
        if (forceScan)
        {
            return new ProjectsConfig()
            {
                Projects = UserDataScanner.ScanUserProjects()
            };
        }
        return UserDataLoader.LoadUserProjects();
    }
}
