using GodotLauncher.Scripts.UiBinding;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class EngineVersion : UiModel
{
    [JsonIgnore]
    public string FileName => System.IO.Path.GetFileName(Path);

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("customIcon")]
    public CustomIcon CustomIcon { get; set; }

    public EngineVersion()
    {
    }

    public EngineVersion(string version, string path, CustomIcon customIcon = null)
    {
        Version = version;
        Path = path;
        CustomIcon = customIcon;
    }
}