using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed class BooleanFilter<TCell> : FilterWidget where TCell : ObjectTable.Cell
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
    private readonly Func<TCell, bool> ObjectValueFunc;
    private readonly ColumnWorker Column;
    public BooleanFilter(Func<TCell, bool> objectValueFunc, ColumnWorker column)
    {
        ObjectValueFunc = objectValueFunc;
        Column = column;
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
    public override bool Eval(Dictionary<ColumnWorker, ObjectTable.Cell> cells)
    {
        return ObjectValueFunc((TCell)cells[Column]) == Value;
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
