using System;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class BooleanFilter : FilterWidget
{
    public override bool IsActive => Value != null;
    public override event Action<FilterWidget>? OnChange;
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
            OnChange?.Invoke(this);
        }
    } = null;
    private readonly Func<ObjectTable.Cell, bool> CellValueFunc;
    public BooleanFilter(Func<ObjectTable.Cell, bool> cellValueFunc)
    {
        CellValueFunc = cellValueFunc;
    }
    protected override Vector2 CalcSize()
    {
        return new Vector2(Text.LineHeight * 2f, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;

        if (Value != true)
        {
            GUI.color = Globals.GUI.TextColorSecondary;
        }

        if (Widgets.Draw.ButtonImageSubtle(rect.CutByX(rect.width / 2f), Verse.Widgets.CheckboxOnTex))
        {
            Value = Value == true ? null : true;
        }

        GUI.color = origGUIColor;

        if (Value != false)
        {
            GUI.color = Globals.GUI.TextColorSecondary;
        }

        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOffTex))
        {
            Value = Value == false ? null : false;
        }

        GUI.color = origGUIColor;
    }
    public override bool Eval(ObjectTable.Cell cell)
    {
        return CellValueFunc(cell) == Value;
    }
    public override void Reset()
    {
        Value = null;
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke(this);
    }
}
