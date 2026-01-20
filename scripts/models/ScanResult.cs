using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class ScanResult
{
    public int Count { get; set; }
    public string Errors { get; set; }

    public ScanResult()
    {
    }

    public ScanResult(int count, string errors)
    {
        Count = count;
        Errors = errors;
    }
}
