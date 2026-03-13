using System.Runtime.CompilerServices;
using Stats.Utils;
using UnityEngine;

namespace Stats.Extensions;

internal static class UnityEngine_Color
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= GUIUtils.Opacity;

        return color;
    }
}
