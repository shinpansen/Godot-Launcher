using GodotLauncher.Scripts.Enums;
using GodotLauncher.Scripts.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class SortTools
{
    public static List<EngineVersion> SortVersionsBestToWorst(List<EngineVersion> versions)
    {
        return versions.OrderByDescending(c => c.Version)
            .ThenByDescending(c => GodotVersionType.Parse(c.Type).Kind)
            .ThenByDescending(c => GodotVersionType.Parse(c.Type).Number ?? int.MinValue)
            .ToList();
    }
}
