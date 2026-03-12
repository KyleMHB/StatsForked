using System;
using LudeonTK;
using UnityEngine;
using Verse;

namespace Stats.Widgets_Legacy;

public static class Draw
{
    internal static bool ButtonTextSubtle(Rect rect, string text, Color textColor, float padHor = 0f)
    {
        var mouseIsOverRect = Mouse.IsOver(rect);
        var origGUIColor = GUI.color;

        if (mouseIsOverRect)
        {
            GUI.color = GenUI.MouseoverColor;
        }

        var wasClicked = Verse.Widgets.ButtonInvisible(rect);

        Verse.Widgets.DrawAtlas(rect, Verse.Widgets.ButtonSubtleAtlas);

        rect.x += padHor;
        rect.width -= padHor * 2f;
        if (mouseIsOverRect)
        {
            rect.x += Globals.GUI.ButtonSubtleContentHoverOffset;
            rect.y -= Globals.GUI.ButtonSubtleContentHoverOffset;
        }

        GUI.color = textColor;
        Verse.Widgets.Label(rect, text);

        GUI.color = origGUIColor;

        return wasClicked;
    }

    internal static bool ButtonTextSubtle(Rect rect, string text, float padHor = 0f)
    {
        return ButtonTextSubtle(rect, text, Color.white, padHor);
    }

    internal static bool ButtonImageSubtle(Rect rect, Texture2D texture, float textureScale = 0.7f)
    {
        var mouseIsOverRect = Mouse.IsOver(rect);
        var origGUIColor = GUI.color;

        if (mouseIsOverRect)
        {
            GUI.color = GenUI.MouseoverColor;
        }

        var wasClicked = Verse.Widgets.ButtonInvisible(rect);

        Verse.Widgets.DrawAtlas(rect, Verse.Widgets.ButtonSubtleAtlas);

        if (mouseIsOverRect)
        {
            rect.x += Globals.GUI.ButtonSubtleContentHoverOffset;
            rect.y -= Globals.GUI.ButtonSubtleContentHoverOffset;
        }

        Verse.Widgets.DrawTextureFitted(rect, texture, textureScale);

        GUI.color = origGUIColor;

        return wasClicked;
    }

    public static void Label(Rect rect, string text, GUIStyle style)
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
    }
}
