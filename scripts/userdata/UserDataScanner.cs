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

    public static List<EngineVersion> ScanUserEngines(out string errors)
    {
        var settings = UserDataLoader.LoadUserSettings();
        settings.CustomInstallsDirectories.Add(new FileSystemPath(GodotDownloader.DownloadPath));
        List<EngineVersion> engines = [];
        HashSet<string> files = ScanFiles(settings.CustomInstallsDirectories, "exe", out errors);
        
        foreach (string file in files)
        {
            string fileFullName = new FileInfo(file).FullName;
            if (settings.ExcludedFiles.Any(f => new FileInfo(f.FullName).FullName == fileFullName))
                continue;

            FileVersionInfo info = FileVersionInfo.GetVersionInfo(file);
            if (string.IsNullOrEmpty(info.ProductName) || 
                !info.ProductName.ToLower().Equals(GodotEngineName)) continue;

            if(!engines.Any(e => new FileInfo(e.Path).FullName == new FileInfo(file).FullName))
                engines.Add(new Models.EngineVersion(info.FileVersion, file));
        }
        return engines;
    }

    public static List<Project> ScanUserProjects(out string errors)
    {
        var settings = UserDataLoader.LoadUserSettings();
        List<Project> projects = [];
        HashSet<string> files = ScanFiles(settings.ProjectsDirectories, "godot", out errors);

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
            DateTime? lastEdit = new FileInfo(file).LastWriteTime;

            bool cSharp = false;
            string version = string.Empty;
            var icon = config.GetValue("application", "config/icon", string.Empty).AsString();
            var features4X = config.GetValue("application", "config/features", string.Empty);
            var config3X2X = config.GetValue("", "config_version", 0);
            var mono3X2X = config.GetValue("mono", "project/assembly_name", "");
            if (!string.IsNullOrEmpty(features4X.AsString()))
            {
                cSharp = features4X.AsString().ToLower().Contains("c#");
                version = features4X.AsStringArray().FirstOrDefault();
            }
            else if (config3X2X.AsInt32() > 0)
            {
                version = config3X2X.AsInt32() >= 4 ? "3.X" : "2.X";
                cSharp = !string.IsNullOrEmpty(mono3X2X.AsString());
            }
            else 
                version = "1.x";

            projects.Add(new Project(name, path, lastEdit, icon, version, cSharp));
        }

        return projects;
    }

    public static List<Project> MatchAvailableVersions(List<Project> projects, List<EngineVersion> versions)
    {
        List<Project> projectsUpdated = [];
        if (projects is null) return projectsUpdated;

        foreach (var p in projects)
        {
            var projectEngineVersion = new EngineVersion(p.Version, "", "", p.CSharp);
            var versionsMatched = versions.Where(v => MatchVersionForProject(projectEngineVersion, v)).ToList();

            if (!System.Environment.Is64BitOperatingSystem)
                versionsMatched.RemoveAll(v => v.ExeBitness == Enums.ExeBitness.x64);

            if(versionsMatched.Any()) 
                versionsMatched = SortTools.SortVersionsBestToWorst(versionsMatched);

            p.AvailableVersions = versionsMatched ?? [];

            if (versionsMatched.Any())
            {
                var v = versionsMatched.First();
                p.OptimalLaunchVersion = new EngineVersion(v.Version, v.Path, v.Type, v.Mono, v.CustomIcon);
            }
            else
                p.OptimalLaunchVersion = null;

            if (p.DefaultLaunchVersion is not null && !File.Exists(p.DefaultLaunchVersion.Path))
                p.DefaultLaunchVersion = null;

            projectsUpdated.Add(p);
        }
        return projectsUpdated;
    }

    private static bool MatchVersionForProject(
        EngineVersion projectEngineVersion,
        EngineVersion engineVersionTested)
    {
        var projectVersion = new Version(projectEngineVersion.Version.Replace("X", "0"));
        var versionTested = new Version(engineVersionTested.Version);

        bool versionOk;
        if (projectVersion.Major >= 4)
        {
            versionOk = projectVersion.Major == versionTested.Major &&
                projectVersion.Minor == versionTested.Minor;
        }
        else
            versionOk = projectVersion.Major == versionTested.Major;

        if (!versionOk) return false;
        else if (projectEngineVersion.Mono == true) return engineVersionTested.Mono == true;
        return true;
    }

    private static HashSet<string> ScanFiles(List<FileSystemPath> paths, string extension, out string errors)
    {
        HashSet<string> files = [];
        errors = string.Empty;
        if (extension.StartsWith(".")) extension = extension.Substring(1);
        foreach (var d in paths.Select(d => d.FullName))
        {
            if (!System.IO.Directory.Exists(d)) continue;
            try
            {
                var scannedFiles = Directory.EnumerateFiles(d, $"*.{extension}", SearchOption.AllDirectories);
                foreach (var f in scannedFiles) files.Add(f);
            }
            catch (Exception ex)
            {
                if (!string.IsNullOrEmpty(errors)) errors += System.Environment.NewLine;
                errors += ($"Path '{d}' not scanned: {ex.Message}");
            }
        }
        return files;
    }
}
