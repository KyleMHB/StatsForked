using System.Runtime.CompilerServices;
using Stats.Utils;
using UnityEngine;

namespace Stats.Extensions;

public static class UnityEngine_Color
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= GUIUtils.Opacity;

        return color;
    }
}
