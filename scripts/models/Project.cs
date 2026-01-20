using Godot;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class Project
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("lastEdit")]
    public DateTime? LastEdit { get; set; }

    [JsonPropertyName("iconPath")]
    public string IconPath { get; set; }

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("cSharp")]
    public bool CSharp { get; set; }

    [JsonPropertyName("launchArguments")]
    public string LaunchArguments { get; set; } = string.Empty;

    [JsonPropertyName("defaultLaunchVersion")]
    public EngineVersion DefaultLaunchVersion { get; set; }

    [JsonIgnore]
    public EngineVersion OptimalLaunchVersion { get; set; }

    [JsonIgnore]
    public List<EngineVersion> AvailableVersions { get; set; } = [];

    [JsonIgnore]
    public EngineVersion LaunchVersion => DefaultLaunchVersion ?? (OptimalLaunchVersion ?? new());

    [JsonIgnore]
    public bool CanEdit => OptimalLaunchVersion != null;

    [JsonIgnore]
    public string VersionCompatible => Version + (CSharp ? " Mono" : "") +
        (!System.Environment.Is64BitOperatingSystem ? " (x86)" : "");

    public Project()
    {
    }

    public Project(string name, string path, DateTime? lastEdit, string iconPath, string version, bool cSharp)
    {
        Name = name;
        Path = path;
        LastEdit = lastEdit;
        IconPath = iconPath;
        Version = version;
        CSharp = cSharp;
    }
}
