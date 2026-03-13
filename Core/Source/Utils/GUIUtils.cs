using System;
using System.Runtime.CompilerServices;
using LudeonTK;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Utils;

public static class GUIUtils
{
    internal static float Opacity = 1f;

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= Opacity;

        return color;
    }

    public static Rect Label(this Rect rect, string text, GUIStyle style)
    {
        float num = Prefs.UIScale / 2f;
        if (Prefs.UIScale > 1f && Math.Abs(num - Mathf.Floor(num)) > float.Epsilon)
        {
            rect.xMin = UIScaling.AdjustCoordToUIScalingFloor(rect.xMin);
            rect.yMin = UIScaling.AdjustCoordToUIScalingFloor(rect.yMin);
            rect.xMax = UIScaling.AdjustCoordToUIScalingCeil(rect.xMax + 1E-05f);
            rect.yMax = UIScaling.AdjustCoordToUIScalingCeil(rect.yMax + 1E-05f);
        }
        GUI.Label(rect, text, style);

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

    internal static bool ButtonTextSubtle(this Rect rect, string text, Color textColor, float padHor = 0f)
    {
        bool mouseIsOverRect = Mouse.IsOver(rect);
        Color origGUIColor = GUI.color;

        if (mouseIsOverRect)
        {
            GUI.color = GenUI.MouseoverColor;
        }

        bool wasClicked = Verse.Widgets.ButtonInvisible(rect);

        Verse.Widgets.DrawAtlas(rect, Verse.Widgets.ButtonSubtleAtlas);

        rect.x += padHor;
        rect.width -= padHor * 2f;
        if (mouseIsOverRect)
        {
            rect.x += GUIStyles.Global.ButtonSubtleContentHoverOffset;
            rect.y -= GUIStyles.Global.ButtonSubtleContentHoverOffset;
        }

        GUI.color = textColor;
        Verse.Widgets.Label(rect, text);

        GUI.color = origGUIColor;

        return wasClicked;
    }

    internal static bool ButtonTextSubtle(this Rect rect, string text, float padHor = 0f)
    {
        return ButtonTextSubtle(rect, text, Color.white, padHor);
    }

    internal static bool ButtonImageSubtle(this Rect rect, Texture2D texture, float textureScale = 0.7f)
    {
        bool mouseIsOverRect = Mouse.IsOver(rect);
        Color origGUIColor = GUI.color;

        if (mouseIsOverRect)
        {
            GUI.color = GenUI.MouseoverColor;
        }

        bool wasClicked = Verse.Widgets.ButtonInvisible(rect);

        Verse.Widgets.DrawAtlas(rect, Verse.Widgets.ButtonSubtleAtlas);

        if (mouseIsOverRect)
        {
            rect.x += GUIStyles.Global.ButtonSubtleContentHoverOffset;
            rect.y -= GUIStyles.Global.ButtonSubtleContentHoverOffset;
        }

        Verse.Widgets.DrawTextureFitted(rect, texture, textureScale);

        GUI.color = origGUIColor;

        return wasClicked;
    }
}
