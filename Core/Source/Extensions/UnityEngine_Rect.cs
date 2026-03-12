using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

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
        return rect.ContractedBy(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
    }

    internal static Rect ScaledBy(this Rect rect, float scale)
    {
        if (scale != 1f)
        {
            float scaledWidth = rect.width * scale;
            float scaledHeight = rect.height * scale;
            rect.x += (rect.width - scaledWidth) / 2f;
            rect.y += (rect.height - scaledHeight) / 2f;
            rect.width = scaledWidth;
            rect.height = scaledHeight;
        }

        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTexture(this Rect rect, Texture2D texture)
    {
        return rect.DrawTexture(texture, Color.white);
    }

    internal static Rect DrawTexture(this Rect rect, Texture2D texture, Color color, ScaleMode scaleMode = ScaleMode.StretchToFill)
    {
        GUI.DrawTexture(
            rect,
            texture,
            scaleMode,
            true,
            0f,
            color,
            Vector2.zero,
            Vector2.zero
        );

        return rect;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTexture(this Rect rect, Texture2D texture, float scale)
    {
        return rect.DrawTexture(texture, Color.white, scale);
    }

    internal static Rect DrawTexture(this Rect rect, Texture2D texture, Color color, float scale)
    {
        rect.ScaledBy(scale).DrawTexture(texture, color, ScaleMode.ScaleToFit);

        return rect;
    }

    internal static void DrawBorderBottom(this Rect rect, Color color)
    {
        rect.y = rect.yMax - 1f;
        rect.height = 1f;
        rect.Fill(color);
    }

    internal static void DrawBorderRight(this Rect rect, Color color)
    {
        rect.x = rect.xMax - 1f;
        rect.width = 1f;
        rect.Fill(color);
    }

    internal static Rect Fill(this Rect rect, Color color)
    {
        return rect.DrawTexture(BaseContent.WhiteTex, color);
    }

    internal static Rect Highlight(this Rect rect)
    {
        return rect.DrawTexture(TexUI.HighlightTex);
    }

    internal static Rect HighlightLight(this Rect rect)
    {
        return rect.DrawTexture(Verse.Widgets.LightHighlight);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect HighlightStrong(this Rect rect, Color color)
    {
        Verse.Widgets.DrawStrongHighlight(rect);

        return rect;
    }

    internal static Rect HighlightSelected(this Rect rect)
    {
        return rect.DrawTexture(TexUI.HighlightSelectedTex);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect Tip(this Rect rect, TipSignal tip)
    {
        TooltipHandler.TipRegion(rect, tip);

        return rect;
    }

    internal static bool ButtonGhostly(this Rect rect)
    {
        if (Event.current.IsRepaint() && Mouse.IsOver(rect))
        {
            MouseoverSounds.DoRegion(rect);
            rect.DrawTexture(TexUI.HighlightTex);
        }

        return GUI.Button(rect, "", Verse.Widgets.EmptyStyle);
    }
}
