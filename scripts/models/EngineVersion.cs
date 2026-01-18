using GodotLauncher.Scripts.Tools;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;

namespace GodotLauncher.Scripts.Models;

public class EngineVersion
{
    [JsonIgnore]
    public string FileName => System.IO.Path.GetFileName(Path);

    [JsonIgnore]
    public string FileNameTrim => FileName?.Length > 25 ? FileName.Substring(0, 22) + "..." : FileName;

    [JsonPropertyName("version")]
    public string Version { get; set; }

    [JsonPropertyName("path")]
    public string Path { get; set; }

    [JsonPropertyName("type")]
    public string Type { get; set; }

    [JsonPropertyName("mono")]
    public bool? Mono { get; set; }

    [JsonPropertyName("customIcon")]
    public CustomIcon CustomIcon { get; set; }

    [JsonIgnore]
    public string FormatedName => $"{Version} {Type ?? ""}".TrimEnd() + (Mono == true ? " (Mono)" : "");

    public EngineVersion()
    {
    }

    public EngineVersion(
        string version, 
        string path, 
        string type = "",
        bool? mono = null,
        CustomIcon customIcon = null)
    {
        Version = version;
        Path = path;
        Type = type;
        Mono = mono;
        CustomIcon = customIcon ?? new();
        UpdateTypeAndMono();
    }

    private void UpdateTypeAndMono()
    {
        if (string.IsNullOrEmpty(FileName)) return;

        string[] fileSplit = FileName.Split('-');
        if(fileSplit.Length > 1)
        {
            string typeFull = fileSplit[1];
            string[] typeSplit = typeFull.Split('_');

            if (string.IsNullOrEmpty(Type))
                Type = StringTools.FirstLetterUpper(typeSplit[0]);

            if (Mono is null && typeSplit.Length > 1)
                Mono = typeSplit[1].ToLower() == "mono";
        }
        Mono ??= false;

        if (string.IsNullOrEmpty(Type)) Type = "???";
    }
}