using GodotLauncher.Scripts.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;

namespace GodotLauncher.Scripts.Models;

public readonly struct GodotVersionType
{
    public GodotVersionKind Kind { get; }
    public int? Number { get; }

    public GodotVersionType(GodotVersionKind kind, int? number)
    {
        Kind = kind;
        Number = number;
    }

    public static GodotVersionType Parse(string input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return new GodotVersionType(GodotVersionKind.Unknown, null);

        input = input.ToLowerInvariant();

        if (input == "stable")
            return new GodotVersionType(GodotVersionKind.Stable, null);

        var match = Regex.Match(input, @"^(alpha|dev|beta|rc)(\d+)?$");
        if (!match.Success)
            return new GodotVersionType(GodotVersionKind.Unknown, null);

        var kind = match.Groups[1].Value switch
        {
            "alpha" => GodotVersionKind.Alpha,
            "dev" => GodotVersionKind.Dev,
            "beta" => GodotVersionKind.Beta,
            "rc" => GodotVersionKind.Rc,
            _ => GodotVersionKind.Unknown
        };

        int? number = match.Groups[2].Success
            ? int.Parse(match.Groups[2].Value)
            : null;

        return new GodotVersionType(kind, number);
    }
}
