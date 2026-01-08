using Godot;
using GodotLauncher.Scripts.Enums;
using GodotLauncher.Scripts.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class Settings
{
    [JsonPropertyName("customInstallsDirectories")]
    public List<FileSystemPath> CustomInstallsDirectories { get; set; } = [];

    [JsonPropertyName("projectsDirectories")]
    public List<FileSystemPath> ProjectsDirectories { get; set; } = [];

    [JsonPropertyName("defaultInstallDirectory")]
    public string DefaultInstallDirectory { get; set; } = ProjectSettings.GlobalizePath("user://");

    [JsonPropertyName("excludedFiles")]
    public List<FileSystemPath> ExcludedFiles { get; set; } = [];

    [JsonPropertyName("closeLauncherWhenStartingGodot")]
    public bool CloseLauncherWhenStartingGodot { get; set; } = true;

    [JsonPropertyName("scanVersionsWhenLauncherStart")]
    public bool ScanVersionsWhenLauncherStart { get; set; }

    [JsonPropertyName("appTheme")]
    public AppTheme AppTheme { get; set; }

    [JsonPropertyName("test")]
    public string Test { get; set; } = "gloubi";

    public Settings()
    {
        //TODO : Handle linux and mac os
        if (CustomInstallsDirectories.Count == 0)
        {
            CustomInstallsDirectories.Add(new FileSystemPath(@"C:\Program Files (x86)\Godot"));
        }
    }
}