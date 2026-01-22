using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class DownloadRequest
{
    public string Version { get; set; }
    public string Type { get; set; }
    public bool Moono { get; set; }
    public bool Version32bits { get; set; }

    public DownloadRequest(string version, string type, bool moono, bool version32bits)
    {
        Version = version;
        Type = type;
        Moono = moono;
        Version32bits = version32bits;
    }
}
