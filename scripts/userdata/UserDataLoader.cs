using Godot;
using GodotLauncher.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UserData;

public static class UserDataLoader
{
    public const string VersionsFileName = "versions.json";

    public const string SettingsFileName = "settings.json";
    public const string ProjectsFileName = "projects.json";

    public static VersionsConfig LoadUserVersions() => LoadConfigFile<VersionsConfig>(VersionsFileName);

    public static ProjectsConfig LoadUserProjects() => LoadConfigFile<ProjectsConfig>(ProjectsFileName);

    public static Settings LoadUserSettings() => LoadConfigFile<Settings>(SettingsFileName);

    public static VersionsConfig MergeUserVersionsConfig(VersionsConfig config, List<Models.EngineVersion> enginesScanned)
    {
        List<Models.EngineVersion> enginesUpdated = [];
        foreach (var engine in enginesScanned)
        {
            var engineName = new System.IO.FileInfo(engine.FileName).Name;
            var matchedEngine = config.Versions.FirstOrDefault(
                e => new System.IO.FileInfo(e.FileName).Name == engineName &&
                e.Version == e.Version);

            if (matchedEngine is not null)
                engine.CustomIcon = matchedEngine.CustomIcon;
            enginesUpdated.Add(engine);
        }

        config.Versions = enginesUpdated;
        return config;
    }

    public static ProjectsConfig MergeUserProjectsConfig(ProjectsConfig config, List<Models.Project> projectsScanned)
    {
        List<Models.Project> projectsUpdated = [];
        foreach (var project in projectsScanned)
        {
            var matchedProject = config.Projects?.FirstOrDefault(
                p => p.Name == project.Name && 
                p.Version == project.Version);

            if (matchedProject is not null)
            {
                project.LaunchArguments = matchedProject.LaunchArguments;
                project.DefaultLaunchVersion = matchedProject.DefaultLaunchVersion;
            }
            projectsUpdated.Add(project);
        }

        config.Projects = projectsUpdated;
        return config;
    }

    public static void SaveConfig<T>(string configFileName, T config)
    {
        using var file = FileAccess.Open("user://" + configFileName, FileAccess.ModeFlags.Write);
        file.StoreString(System.Text.Json.JsonSerializer.Serialize(config));
    }

    private static T LoadConfigFile<T>(string configFileName) where T : new()
    {
        using var file = FileAccess.Open("user://" + configFileName, FileAccess.ModeFlags.Read);
        if (file is null)
        {
            Error error = FileAccess.GetOpenError();
            if (error == Error.FileNotFound)
            {
                T newConfig = new();
                SaveConfig(configFileName, newConfig);
                return newConfig;
            }
            else throw new System.IO.FileLoadException(error.ToString());
        }

        string content = file.GetAsText();
        T config = System.Text.Json.JsonSerializer.Deserialize<T>(content);
        return config;
    }
}
