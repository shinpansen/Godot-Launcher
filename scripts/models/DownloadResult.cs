using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class DownloadResult
{
    public bool Success { get; set; }
    public string Errors { get; set; }

    public DownloadResult()
    {
    }

    public DownloadResult(bool success, string errors = "")
    {
        Success = success;
        Errors = errors;
    }
}
