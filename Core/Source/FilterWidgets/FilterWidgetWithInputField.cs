using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.FilterWidgets;

public abstract class FilterWidgetWithInputField<TLhs, TRhs> : FilterWidget
{
    private Vector2 OperatorButtonSize;
    private const float OperatorButtonMinWidth = 24f;
    private const float OperatorButtonPaddingHor = Globals.GUI.PadXs;
    private const float InputFieldMinWidth = OperatorButtonMinWidth * 2f;
    private readonly FloatMenu OperatorsMenu;
    protected abstract RelOperator<TLhs, TRhs> Operator { get; set; }
    private readonly string Placeholder;
    protected abstract string InputFieldText { get; }
    private bool InputFieldIsEmpty => InputFieldText.Length == 0;
    private readonly Widget ClearButton;
    protected FilterWidgetWithInputField(IEnumerable<RelOperator<TLhs, TRhs>> operators, string? placeholder = null)
    {
        Placeholder = placeholder ?? "";

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
        ClearButton = new Icon(TexButton.CloseXSmall, 0.5f)
        .HoverColor(Globals.GUI.TextColorSecondary);
    }
    public sealed override Vector2 GetSize()
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
        var clearButtonRect = rect.RightPartPixels(ClearButton.GetSize().x);

        if
        (
            InputFieldIsEmpty == false
            && Event.current.type == EventType.MouseDown
            && Mouse.IsOver(clearButtonRect)
        )
        {
            ClearInputField();
            Event.current.Use();
        }

        DrawInputField(rect);

        if (InputFieldIsEmpty && IsActive == false)
        {
            rect.xMin += Globals.GUI.EstimatedInputFieldInnerPadding;
            Verse.Widgets.Label(rect, Placeholder);
        }
        else
        {
            MouseoverSounds.DoRegion(clearButtonRect);
            ClearButton.DrawIn(clearButtonRect);
        }

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
    private Vector2 CalcInputFieldSize()
    {
        Vector2 size;

        if (InputFieldIsEmpty)
        {
            size = Text.CalcSize(Placeholder);
            size.x += Globals.GUI.EstimatedInputFieldInnerPadding * 2f;
        }
        else
        {
            size = Text.CalcSize(InputFieldText);
            size.x += Globals.GUI.Pad + ClearButton.GetSize().x;
        }

        if (size.x < InputFieldMinWidth)
        {
            size.x = InputFieldMinWidth;
        }

        return size;
    }
    protected abstract void DrawInputField(Rect rect);
    protected abstract void ClearInputField();
}
