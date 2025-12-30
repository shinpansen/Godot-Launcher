using Godot;
using GodotLauncher.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UserData;

public static class UserDataLoader
{
    public static VersionsConfig LoadUserVersions(string configFileName) => LoadConfigFile<VersionsConfig>(configFileName);
    public static void SaveUserVersions(VersionsConfig config, string configFileName)
    {
        using var file = FileAccess.Open("user://" + configFileName, FileAccess.ModeFlags.Write);
        file.StoreString(System.Text.Json.JsonSerializer.Serialize(config));
    }

    public static VersionsConfig MergeUserVersionsConfig(VersionsConfig config, List<Models.EngineVersion> enginesScanned)
    {
        List<Models.EngineVersion> enginesUpdated = [];
        foreach (var engine in enginesScanned)
        {
            var matchedEngine = config.Versions.FirstOrDefault(e => e.Path == engine.Path && e.Version == engine.Version);
            if (matchedEngine is null) enginesUpdated.Add(engine);
            else enginesUpdated.Add(matchedEngine);
        }

        config.Versions = enginesUpdated;
        return config;
    }

    private static T LoadConfigFile<T>(string configFileName) where T : new()
    {
        using var file = FileAccess.Open("user://" + configFileName, FileAccess.ModeFlags.Read);
        if (file is null)
        {
            Error error = FileAccess.GetOpenError();
            if (error == Error.FileNotFound) return new();
            else throw new System.IO.FileLoadException(error.ToString());
        }

        string content = file.GetAsText();
        T config = System.Text.Json.JsonSerializer.Deserialize<T>(content);
        return config;
    }
}
