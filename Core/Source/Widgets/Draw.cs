using System.Reflection;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets;

public static class Draw
{
    private static readonly FieldInfo DialogInfoCardStuffField =
        typeof(Dialog_InfoCard)
        .GetField("stuff", BindingFlags.Instance | BindingFlags.NonPublic);

    public static void DefInfoDialog(Def def, ThingDef? stuff = null)
    {
        var dialog = new Dialog_InfoCard(def);

        if (stuff != null)
        {
            DialogInfoCardStuffField.SetValue(dialog, stuff);
        }

        Find.WindowStack.Add(dialog);
    }

    internal static void VerticalLine(float x, float y, float length, Color color)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        var origGUIColor = GUI.color;
        GUI.color = color;
        Verse.Widgets.DrawLineVertical(x, y, length);
        GUI.color = origGUIColor;
    }

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

    internal static bool ButtonGhostly(Rect rect)
    {
        if (Event.current.type == EventType.Repaint && Mouse.IsOver(rect))
        {
            MouseoverSounds.DoRegion(rect);
            GUI.DrawTexture(rect, TexUI.HighlightTex, ScaleMode.StretchToFill, true, 0f, Color.white, 0f, 0f);
        }

        return GUI.Button(rect, "", Verse.Widgets.EmptyStyle);
    }
}
