using GodotLauncher.Scripts.Models;
using GodotLauncher.Scripts.UserData;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class GodotDownloader
{
    public static List<Release> DownloadReleasesInfo()
    {
        Settings settings = UserDataLoader.LoadUserSettings();
        var url = settings.VersionUrl;

        var json = new WebClient().DownloadString(url);
        return System.Text.Json.JsonSerializer.Deserialize<List<Release>>(json);
    }
}
