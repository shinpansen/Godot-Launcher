using Godot;
using GodotLauncher.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.UserData;

public static class UserDataScanner
{
    public const string GodotEngineName = "godot engine";

    public static List<Models.EngineVersion> ScanUserEngines()
    {
        var settings = UserDataLoader.LoadUserSettings();
        List<Models.EngineVersion> engines = [];
        List<string> files = [];
        foreach(var d in settings.CustomInstallDirectories.Select(d => d.FullName))
        {
            if (!System.IO.Directory.Exists(d)) continue;
            var scannedFiles = Directory.EnumerateFiles(d, "*.exe", SearchOption.AllDirectories);
            files.AddRange(scannedFiles);
        }
        
        foreach (string file in files)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(file);
            if (string.IsNullOrEmpty(info.ProductName) || 
                !info.ProductName.ToLower().Equals(GodotEngineName)) continue;

            engines.Add(new Models.EngineVersion(info.FileVersion, file));
        }

        return engines;
    }
}
