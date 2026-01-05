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
        HashSet<string> files = [];
        foreach(var d in settings.CustomInstallsDirectories.Select(d => d.FullName))
        {
            if (!System.IO.Directory.Exists(d)) continue;
            var scannedFiles = Directory.EnumerateFiles(d, "*.exe", SearchOption.AllDirectories);
            foreach(var f in scannedFiles) files.Add(f);
        }
        
        foreach (string file in files)
        {
            FileVersionInfo info = FileVersionInfo.GetVersionInfo(file);
            if (string.IsNullOrEmpty(info.ProductName) || 
                !info.ProductName.ToLower().Equals(GodotEngineName)) continue;

            if(!engines.Any(e => new FileInfo(e.Path).FullName == new FileInfo(file).FullName))
                engines.Add(new Models.EngineVersion(info.FileVersion, file));
        }
        return engines;
    }
}
