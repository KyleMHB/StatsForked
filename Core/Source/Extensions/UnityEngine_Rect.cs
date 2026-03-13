using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats.Extensions;

public static class UnityEngine_Rect
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect CutByX(ref this Rect rect, float amount)
    {
        Rect result = rect with { width = amount };
        // Changing "xMin" also auto corrects width. Changing "x" doesn't.
        rect.xMin += amount;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect CutByY(ref this Rect rect, float amount)
    {
        Rect result = rect with { height = amount };
        // Changing "yMin" also auto corrects height. Changing "y" doesn't.
        rect.yMin += amount;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect ContractedByObjectTableCellPadding(this Rect rect)
    {
        return rect.ContractedBy(GUISkin.TableCell.PadHor, GUISkin.TableCell.PadVer);
    }
}
