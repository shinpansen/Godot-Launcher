using GodotLauncher.Scripts.Enums;
using GodotLauncher.Scripts.Binding;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class DownloadConfig
{
    public List<Release> AvailableReleases { get; set; } = [];
    public List<string> AvailableTypes { get; set; } = [];
    public bool MonoAvailable { get; set; }

    public DownloadConfig()
    {
    }
}
