using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class Release
{
    [JsonPropertyName("name")]
    public string Name { get; set; }

    [JsonPropertyName("releases")]
    public List<ReleaseVersion> Releases { get; set; }
}
