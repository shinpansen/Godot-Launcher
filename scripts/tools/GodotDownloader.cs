using Godot;
using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class GodotDownloader
{
    public static string DownloadPath => ProjectSettings.GlobalizePath("user://") + "downloads";

    private static readonly System.Net.Http.HttpClient _httpClient = new();

    public static List<Release> DownloadReleasesInfo()
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        var url = settings.VersionUrl;

        //TODO - Find why it doesn't work with HttpClient
        var json = new WebClient().DownloadString(url);
        return System.Text.Json.JsonSerializer.Deserialize<List<Release>>(json);
    }

    public static List<DownloadUrl> GetGodotArchiveDownloadUrls(string version, string type)
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        var url = settings.DownloadUrl;
        url = string.Format(url, $"{version}-{type}");


        string html = _httpClient.GetStringAsync(url).Result;
        string pattern = @"href=""(https://downloads\.godotengine\.org/\?version=[^\""]+)""";

        MatchCollection matches = Regex.Matches(html, pattern);
        List<DownloadUrl> downloadUrls = [];
        foreach (Match match in matches)
        {
            if (match.Groups.Count <= 1) continue;

            string urlValue = match.Groups[1].Value;
            Dictionary<string, string> urlParameters = ParseQuery(urlValue);
            downloadUrls.Add(new DownloadUrl(
                urlValue,
                urlParameters["version"],
                urlParameters["flavor"],
                urlParameters["slug"],
                urlParameters["platform"]
            ));
        }
        return downloadUrls;
    }

    public static string FilterUrls(List<DownloadUrl> urls, string version, string type, bool mono, bool version32bits)
    {
        string platform;
        if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            platform = "windows" + (version32bits ? ".32" : ".64");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            platform = "linux" + (version32bits ? ".32" : ".64");
        else if (RuntimeInformation.IsOSPlatform(OSPlatform.OSX))
            platform = "macos.universal";
        else
            throw new Exception("Current OS non supported");

        var urlsFiltered = urls.Where(u => u.Platform == platform)
            .Where(u => u.Version == version)
            .Where(u => u.Flavor == type);

        if (new Version(version).Major >= 3)
        {
            if (mono) urlsFiltered = urlsFiltered.Where(u => u.Slug.StartsWith("mono"));
            else urlsFiltered = urlsFiltered.Where(u => !u.Slug.StartsWith("mono"));
        }
        return urlsFiltered.FirstOrDefault()?.Url;
    }

    public static string DownloadGodotZip(string url, string version, string type, bool mono, bool version32bits)
    {
        string zipName = $"Godot_v{version}-{type}.zip";
        string path = $"{DownloadPath}/{version}/{type}";
        path += version32bits ? "/x86" : "/x64";
        if (mono) path += "/mono";

        if(!System.IO.Directory.Exists(path))
            System.IO.Directory.CreateDirectory(path);

        string outputPath = $"{path}/{zipName}";
        using var client = new WebClient(); //TODO use HttpClient
        client.DownloadFile(url, outputPath);
        return outputPath;
    }

    public static void ExtractZip(string zipPath, bool deleteZip = true)
    {
        string extractPath = System.IO.Path.GetDirectoryName(zipPath);
        ZipFile.ExtractToDirectory(zipPath, extractPath, overwriteFiles: true);
        if(deleteZip) System.IO.File.Delete(zipPath);
    }

    private static Dictionary<string, string> ParseQuery(string url)
    {
        var uri = new Uri(url);
        var query = uri.Query.TrimStart('?').Split('&');

        var result = new Dictionary<string, string>();
        foreach (var part in query)
        {
            var kv = part.Split('=', 2);
            if (kv.Length == 2)
            {
                result[Uri.UnescapeDataString(kv[0])] = Uri.UnescapeDataString(kv[1]);
            }
        }
        return result;
    }
}
