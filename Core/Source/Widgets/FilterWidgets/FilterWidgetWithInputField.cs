using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal abstract class FilterWidgetWithInputField<TObject, TExprLhs, TExprRhs> : FilterWidget<TObject, TExprLhs, TExprRhs>
    where TExprRhs : notnull
{
    private Vector2 OperatorButtonSize;
    private const float OperatorButtonMinWidth = 24f;
    private const float OperatorButtonPadding = Globals.GUI.PadXs;
    private Vector2 InputFieldSize;
    private const float InputFieldMinWidth = OperatorButtonMinWidth * 2f;
    private bool ChildrenSizesAreCached = false;
    private readonly FloatMenu OperatorsMenu;
    protected FilterWidgetWithInputField(
        Func<TObject, TExprLhs> lhs,
        TExprRhs rhs,
        IEnumerable<AbsOperator> operators,
        AbsOperator? defaultOperator = null
    ) : base(lhs, rhs, defaultOperator ?? operators.First())
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

        OnChange += HandleStateChange;
    }
    protected sealed override Vector2 CalcSize()
    {
        if (ChildrenSizesAreCached == false)
        {
            OperatorButtonSize = CalcOperatorButtonSize();
            InputFieldSize = CalcInputFieldSize();
            ChildrenSizesAreCached = true;
        }

        var size = OperatorButtonSize;
        size.x += InputFieldSize.x;
        size.y = Mathf.Max(size.y, InputFieldSize.y);

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
        size.x += OperatorButtonPadding * 2f;

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

        return Widgets.Draw.ButtonTextSubtle(rect, Operator.Symbol, GUI.color, OperatorButtonPadding);
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
    protected virtual void HandleStateChange(FilterWidget<TObject> _)
    {
        OperatorButtonSize = CalcOperatorButtonSize();
        InputFieldSize = CalcInputFieldSize();

        Resize();
    }
}
