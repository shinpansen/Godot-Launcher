using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Models;

public class ProjectsConfig
{
    [JsonPropertyName("projects")]
    public List<Project> Projects { get; set; } = [];
}
