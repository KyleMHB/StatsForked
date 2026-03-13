using System;
using Stats.Extensions;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Filters;

public sealed class BooleanFilter : FilterWidget
{
    public override bool IsActive => Value != null;
    public override event Action? OnChange;
    private bool? Value
    {
        get => field;
        set
        {
            if (field == value)
            {
                return;
            }

            field = value;
            OnChange?.Invoke();
        }
    } = null;
    private readonly Func<int, bool> CellValueFunc;
    public BooleanFilter(Func<int, bool> cellValueFunc)
    {
        CellValueFunc = cellValueFunc;
    }
    public override Vector2 GetSize()
    {
        return new Vector2(Text.LineHeight * 2f, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;

        if (Value != true)
        {
            GUI.color = GUIUtils.TextColorSecondary;
        }

        if (Widgets_Legacy.Draw.ButtonImageSubtle(rect.CutByX(rect.width / 2f), Verse.Widgets.CheckboxOnTex))
        {
            Value = Value == true ? null : true;
        }

        GUI.color = origGUIColor;

        if (Value != false)
        {
            GUI.color = GUIUtils.TextColorSecondary;
        }

        if (Widgets_Legacy.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOffTex))
        {
            Value = Value == false ? null : false;
        }

        GUI.color = origGUIColor;
    }
    public override bool Eval(int row)
    {
        return CellValueFunc(row) == Value;
    }
    public override void Reset()
    {
        Value = null;
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke();
    }
}
