using Godot;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace GodotLauncher.Scripts.Tools;

public static class VariantTools
{
    public static Variant FromCSharpObject(object value)
    {
        if (value is Variant v)
            return v;

        switch (value)
        {
            case bool b:
                return Variant.From(b);
            case int i:
                return Variant.From(i);
            case long l:
                return Variant.From((int)l);
            case float f:
                return Variant.From(f);
            case double d:
                return Variant.From((float)d);
            case string s:
                return Variant.From(s);
            case StringName sn:
                return Variant.From(sn);
            case Color c:
                return Variant.From(c);
            case Vector2 v2:
                return Variant.From(v2);
            case Vector3 v3:
                return Variant.From(v3);
            case Vector4 v4:
                return Variant.From(v4);
            case Node n:
                return Variant.From(n);
            case Resource r:
                return Variant.From(r);
            case GodotObject go:
                return Variant.From(go);
            case Godot.Collections.Array arr:
                return Variant.From(arr);
            case Godot.Collections.Dictionary dict:
                return Variant.From(dict);
            case IEnumerable enumerable:
                var godotArray = new Godot.Collections.Array();
                foreach (var item in enumerable)
                    godotArray.Add(FromCSharpObject(item));
                return Variant.From(godotArray);
            case Enum e:
                return Variant.From(e.ToString());
            default:
                GD.PushWarning(
                    $"Unsupported Variant conversion: {value.GetType().FullName}"
                );
                return Variant.From(value.ToString());
        }
    }
}
