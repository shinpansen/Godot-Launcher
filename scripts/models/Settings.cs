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
    [JsonPropertyName("language")]
    public Language Language { get; set; }
    
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

    [JsonPropertyName("closeLauncherWhenStartingProject")]
    public bool CloseLauncherWhenStartingProject { get; set; } = true;

    [JsonPropertyName("scanWhenLauncherStart")]
    public bool ScanWhenLauncherStart { get; set; }

    [JsonPropertyName("tabSelect")]
    public TabSelect TabSelect { get; set; }

    [JsonPropertyName("appTheme")]
    public AppTheme AppTheme { get; set; }

    [JsonPropertyName("versionUrl")]
    public string VersionUrl { get; set; } = "https://godotengine.org/versions.json";

    [JsonPropertyName("downloadUrl")]
    public string DownloadUrl { get; set; } = "https://godotengine.org/download/archive/{0}/";

    [JsonIgnore]
    public long LanguageId
    {
        get => (long)Language;
        set
        {
            Language = (Language)value;
        }
    }

    [JsonIgnore]
    public long TabSelectId
    {
        get => (long)TabSelect;
        set
        {
            TabSelect = (TabSelect)value;
        }
    }

    public Settings()
    {
        //TODO : Handle linux and mac os
        if (CustomInstallsDirectories.Count == 0)
        {
            CustomInstallsDirectories.Add(new FileSystemPath(@"C:\Program Files (x86)\Godot"));
        }
    }
}