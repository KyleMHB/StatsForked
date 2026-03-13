using System.Runtime.CompilerServices;
using Stats.Extensions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Utils;

public static class GUIUtils
{
    internal const float EstimatedInputFieldInnerPadding = 2f;
    internal const float ButtonSubtleContentHoverOffset = 2f;
    internal const float Pad = 10f;
    internal const float PadSm = 5f;
    internal const float PadXs = 3f;

    public static readonly Color TextColorHighlight = new(1f, 0.98f, 0.62f);
    public static readonly Color TextColorSecondary = Color.grey;
    public static float Opacity { get; set; } = 1f;

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
    internal static Rect DrawTextureFitted(this Rect rect, Texture2D texture)
    {
        return rect.DrawTexture(texture, Color.white, ScaleMode.ScaleToFit);
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect DrawTextureFitted(this Rect rect, Texture2D texture, float scale)
    {
        return rect.DrawTextureFitted(texture, Color.white, scale);
    }

    internal static Rect DrawTextureFitted(this Rect rect, Texture2D texture, Color color, float scale)
    {
        Rect scaledRect;
        if (scale != 1f)
        {
            scaledRect = rect.ScaledBy(scale);
        }
        else
        {
            scaledRect = rect;
        }
        scaledRect.DrawTexture(texture, color, ScaleMode.ScaleToFit);

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
        MouseoverSounds.DoRegion(rect);

        if (Event.current.IsRepaint() && Mouse.IsOver(rect))
        {
            rect.Highlight();
        }

        return GUI.Button(rect, "", Verse.Widgets.EmptyStyle);
    }
}
