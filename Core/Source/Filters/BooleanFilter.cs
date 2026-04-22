using System;
using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats.Filters;

public sealed class BooleanFilter : Filter, IPresettableFilter
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
            GUI.color = GUIStyles.Text.ColorSecondary;
        }

        rect = rect.CutLeft(out Rect trueButtonRect, rect.width / 2f);
        if (trueButtonRect.ButtonImageSubtle(Verse.Widgets.CheckboxOnTex))
        {
            Value = Value == true ? null : true;
        }

        GUI.color = origGUIColor;

        if (Value != false)
        {
            GUI.color = GUIStyles.Text.ColorSecondary;
        }

        if (rect.ButtonImageSubtle(Verse.Widgets.CheckboxOffTex))
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

    public string SerializeState()
    {
        return Value?.ToString() ?? "";
    }

    public void DeserializeState(string state)
    {
        if (string.IsNullOrEmpty(state))
        {
            Value = null;
            return;
        }

        Value = bool.Parse(state);
    }
}
