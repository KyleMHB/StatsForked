using System;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats.Utils.Extensions;

public static class UnityEngine_Rect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete]
    internal static Rect CutByX(ref this Rect rect, float amount)
    {
        Rect result = rect with { width = amount };
        rect.xMin += amount;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect CutByX(this Rect rect, out Rect result, float amount)
    {
        result = rect with { width = amount };

        rect.xMin += amount;
        return rect;
    }

    internal static Rect SkipX(this Rect rect, float amount)
    {
        rect.xMin += amount;
        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    [Obsolete]
    internal static Rect CutByY(ref this Rect rect, float amount)
    {
        Rect result = rect with { height = amount };
        rect.yMin += amount;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect CutByY(this Rect rect, out Rect result, float amount)
    {
        result = rect with { height = amount };

        rect.yMin += amount;
        return rect;
    }

    internal static Rect SkipY(this Rect rect, float amount)
    {
        rect.yMin += amount;
        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect TakeRest(this Rect rect, out Rect result)
    {
        return result = rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect ContractedByObjectTableCellPadding(this Rect rect)
    {
        return rect.ContractedBy(GUIStyles.TableCell.PadHor, GUIStyles.TableCell.PadVer);
    }
}
