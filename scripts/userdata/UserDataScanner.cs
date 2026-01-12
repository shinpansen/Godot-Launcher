using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.Scenes.ProjectsView;
using GodotLauncher.Scripts.Tools;
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

    public static List<EngineVersion> ScanUserEngines()
    {
        var settings = UserDataLoader.LoadUserSettings();
        List<EngineVersion> engines = [];
        HashSet<string> files = ScanFiles(settings.CustomInstallsDirectories, "exe");
        
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

    public static List<Project> ScanUserProjects()
    {
        var settings = UserDataLoader.LoadUserSettings();
        List<Project> projects = [];
        HashSet<string> files = ScanFiles(settings.ProjectsDirectories, "godot");

        foreach (string file in files)
        {
            var config = new ConfigFile();
            var err = config.Load(file);
            if (err != Error.Ok)
            {
                GD.PushWarning($"Failed to load {file}");
                continue;
            }

            string name = config.GetValue("application", "config/name").AsString();
            string path = System.IO.Path.GetDirectoryName(file);

            bool cSharp = false;
            string version = string.Empty;
            var icon = config.GetValue("application", "config/icon", string.Empty).AsString();
            var features4X = config.GetValue("application", "config/features", string.Empty);
            var config3X = config.GetValue("", "config_version", 0);
            if (!string.IsNullOrEmpty(features4X.AsString()))
            {
                cSharp = features4X.AsString().ToLower().Contains("c#");
                version = features4X.AsStringArray().FirstOrDefault();
            }
            else if (config3X.AsInt32() > 0)
            {
                version = config3X.AsInt32() >= 4 ? "3.x" : "2.x";
                //cSharp = TODO;
            }
            else 
                version = "1.x";

            projects.Add(new Project(name, path, icon, version, cSharp));
        }
        return projects;
    }

    private static HashSet<string> ScanFiles(List<FileSystemPath> paths, string extension)
    {
        HashSet<string> files = [];
        if(extension.StartsWith(".")) extension = extension.Substring(1);
        foreach (var d in paths.Select(d => d.FullName))
        {
            if (!System.IO.Directory.Exists(d)) continue;
            var scannedFiles = Directory.EnumerateFiles(d, $"*.{extension}", SearchOption.AllDirectories);
            foreach (var f in scannedFiles) files.Add(f);
        }
        return files;
    }
}
