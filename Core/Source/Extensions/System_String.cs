using System;
using UnityEngine;

namespace Stats.Extensions;

public static class System_String
{
    internal static Color ToUniqueColorRGB(this string str)
    {
        // I don't think that this hash is very reliable.
        uint hash = (uint)str.GetHashCode();
        float r = ((hash & 0xFF000000) >> 24) / 255f;
        float g = ((hash & 0x00FF0000) >> 16) / 255f;
        float b = ((hash & 0x0000FF00) >> 8) / 255f;

        return new Color(r, g, b);
    }

    internal static bool Contains(this string str, string substr, StringComparison comp)
    {
        return str.IndexOf(substr, comp) >= 0;
    }
}
