using GodotLauncher.Scripts.Binding;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class FileSystemPath
{
    [JsonPropertyName("fullName")]
    public string FullName { get; set; }

    [JsonIgnore]
    public string Name => GetName();

    public FileSystemPath()
    {
    }

    public FileSystemPath(string fullName)
    {
        FullName = fullName;
    }

    private string GetName()
    {
        if (string.IsNullOrEmpty(FullName)) return string.Empty;

        if (System.IO.Directory.Exists(FullName))
            return new System.IO.DirectoryInfo(FullName).Name;
        else if (System.IO.File.Exists(FullName))
            return new System.IO.FileInfo(FullName).Name;
        else 
            return string.Empty;
    }
}
