using GodotLauncher.Scripts.Enums;
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

    [JsonPropertyName("sortType")]
    public ProjectSortType SortType { get; set; } = ProjectSortType.LastEdit;

    [JsonPropertyName("sortOrder")]
    public SortOrder SortOrder { get; set; } = SortOrder.Desc;
}
