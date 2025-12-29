using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class CustomIcon
{
    [JsonPropertyName("background")]
    public bool Background { get; set; } = false;

    [JsonPropertyName("grayScale")]
    public bool GrayScale { get; set; } = false;

    [JsonPropertyName("hexColor")]
    public string HexColor { get; set; } = "ffffff";
}
