using GodotLauncher.Scripts.Enums;
using GodotLauncher.Scripts.UiBinding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class VersionsConfig
{
    [JsonPropertyName("versions")]
    public List<EngineVersion> Versions { get; set; } = [];

    [JsonPropertyName("sortType")]
    public EngineSortType SortType { get; set; } = EngineSortType.Version;

    [JsonPropertyName("sortOrder")]
    public SortOrder SortOrder { get; set; } = SortOrder.Desc;

    public VersionsConfig()
    {
    }

    public VersionsConfig(
        List<EngineVersion> versions, 
        EngineSortType sortType = EngineSortType.Version, 
        SortOrder sortOrder = SortOrder.Desc)
    {
        Versions = versions;
        SortType = sortType;
        SortOrder = sortOrder;
    }
}
