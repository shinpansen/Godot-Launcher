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

    public static List<Models.EngineVersion> ScanUserEngines(string directoryPath)
    {
        List<Models.EngineVersion> engines = [];
        IEnumerable<string> files = Directory.EnumerateFiles(directoryPath, "*.exe", SearchOption.AllDirectories);
        
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
