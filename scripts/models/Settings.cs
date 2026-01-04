using Godot;
using GodotLauncher.Scripts.Enums;
using GodotLauncher.Scripts.UiBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class Settings : UiModel
{
    [JsonPropertyName("customInstallDirectories")]
    public List<FileSystemPath> CustomInstallDirectories { get; set; } = [];

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

    public Settings()
    {
        //TODO : Handle linux and mac os
        if (CustomInstallDirectories.Count == 0)
        {
            CustomInstallDirectories.Add(new FileSystemPath(@"C:\Program Files (x86)\Godot"));
        }
    }
}