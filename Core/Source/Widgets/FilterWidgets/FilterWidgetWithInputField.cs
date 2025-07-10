using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal abstract class FilterWidgetWithInputField<TObject, TObjectValue, TValue> : FilterWidget<TObject>
{
    private Vector2 OperatorButtonSize;
    private const float OperatorButtonMinWidth = 24f;
    private const float OperatorButtonPaddingHor = Globals.GUI.PadXs;
    private const float InputFieldMinWidth = OperatorButtonMinWidth * 2f;
    private readonly FloatMenu OperatorsMenu;
    protected abstract RelOperator<TObjectValue, TValue> Operator { get; set; }
    protected FilterWidgetWithInputField(IEnumerable<RelOperator<TObjectValue, TValue>> operators)
    {
        var operatorsMenuOptions = new List<FloatMenuOption>(operators.Count());

        foreach (var @operator in operators)
        {
            var operatorString = @operator.Symbol.Colorize(Globals.GUI.TextColorHighlight);
            var optionLabel = @operator.Description.Length > 0
                ? $"{operatorString} - {@operator.Description}"
                : operatorString;
            var option = new FloatMenuOption(optionLabel, () => Operator = @operator);
            operatorsMenuOptions.Add(option);
        }

        OperatorsMenu = new FloatMenu(operatorsMenuOptions);
    }
    protected sealed override Vector2 CalcSize()
    {
        var size = OperatorButtonSize = CalcOperatorButtonSize();
        var inputFieldSize = CalcInputFieldSize();
        size.x += inputFieldSize.x;
        size.y = Mathf.Max(size.y, inputFieldSize.y);

        return size;
    }
    public sealed override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        var origTextAnchor = Text.Anchor;
        var operatorButtonRect = rect.CutByX(OperatorButtonSize.x);
        var origGUIColor = GUI.color;

        if (IsActive == false)
        {
            GUI.color = Globals.GUI.TextColorSecondary;
        }

        Text.Anchor = TextAnchor.LowerCenter;

        if (DrawOperatorButton(operatorButtonRect))
        {
            Find.WindowStack.Add(OperatorsMenu);
        }

        Text.Anchor = TextAnchor.LowerLeft;

        DrawInputField(rect);

        Text.Anchor = origTextAnchor;
        GUI.color = origGUIColor;
    }
    private Vector2 CalcOperatorButtonSize()
    {
        var size = Text.CalcSize(Operator.Symbol);
        size.x += OperatorButtonPaddingHor * 2f;

        if (size.x < OperatorButtonMinWidth)
        {
            size.x = OperatorButtonMinWidth;
        }

        return size;
    }
    private bool DrawOperatorButton(Rect rect)
    {
        if (Operator.Description.Length > 0 && Mouse.IsOver(rect))
        {
            TooltipHandler.TipRegion(rect, Operator.Description);
        }

        return Widgets.Draw.ButtonTextSubtle(rect, Operator.Symbol, GUI.color, OperatorButtonPaddingHor);
    }
    protected abstract Vector2 CalcInputFieldContentSize();
    private Vector2 CalcInputFieldSize()
    {
        var size = CalcInputFieldContentSize();
        size.x += Globals.GUI.EstimatedInputFieldInnerPadding * 2f;

        if (size.x < InputFieldMinWidth)
        {
            size.x = InputFieldMinWidth;
        }

        return size;
    }
    protected abstract void DrawInputField(Rect rect);
}
