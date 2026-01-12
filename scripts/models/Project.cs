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
    public string VersionMono => Version + (CSharp ? " Mono" : "");

    public Project()
    {
    }

    public Project(string name, string path, string iconPath, string version, bool cSharp)
    {
        Name = name;
        Path = path;
        IconPath = iconPath;
        Version = version;
        CSharp = cSharp;
    }
}
