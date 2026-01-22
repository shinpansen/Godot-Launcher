using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class DownloadUrl
{
    public string Url { get; set; }
    public string Version { get; set; }
    public string Flavor { get; set; }
    public string Slug { get; set; }
    public string Platform { get; set; }

    public DownloadUrl()
    {
    }

    public DownloadUrl(string url, string version, string flavor, string slug, string platform)
    {
        Url = url;
        Version = version;
        Flavor = flavor;
        Slug = slug;
        Platform = platform;
    }
}
