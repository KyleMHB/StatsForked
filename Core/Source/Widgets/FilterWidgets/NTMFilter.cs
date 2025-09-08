using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets;

public abstract class NTMFilter<TLhs, TRhs> : FilterWidget
{
    public override bool IsActive => SelectedOptions.Count > 0;
    // TODO: See if IEnumerable is most fitting type here.
    private readonly IEnumerable<NTMFilterOption<TRhs>> Options;
    private List<NTMFilterOption<TRhs>>? _OptionsList;
    private List<NTMFilterOption<TRhs>> OptionsList => _OptionsList ??= Options.ToList();
    private OptionsWindowWidget? _OptionsWindow;
    private OptionsWindowWidget OptionsWindow => _OptionsWindow ??= new(OptionsList, this, Operators);
    private string? _Info = null;
    private string Info
    {
        get
        {
            if (_Info != null)
            {
                return _Info;
            }

            var stringBuilder = new StringBuilder();

            stringBuilder.AppendInNewLine($"{Operator.Symbol.Colorize(Globals.GUI.TextColorHighlight)} - {Operator.Description}:");

            foreach (var option in OptionsList)
            {
                if (SelectedOptions.Contains(option.Value))
                {
                    stringBuilder.AppendInNewLine($"- {option.Label}");
                }
            }

            return _Info = stringBuilder.ToString();
        }
    }
    private readonly IEnumerable<RelOperator<TLhs, HashSet<TRhs>>> Operators;
    private string ButtonText => IsActive ? Info : ButtonTextWhenInactive;
    private readonly string ButtonTextWhenInactive;
    private const float ButtonMinWidth = 24f;
    private const float ButtonPadHor = Globals.GUI.PadSm;
    private readonly HashSet<TRhs> SelectedOptions = [];
    private RelOperator<TLhs, HashSet<TRhs>> _Operator;
    protected RelOperator<TLhs, HashSet<TRhs>> Operator
    {
        get => _Operator;
        set
        {
            if (_Operator == value)
            {
                return;
            }

            _Operator = value;
            _Info = null;
            Resize();
            OnChange?.Invoke();
        }
    }
    private readonly RelOperator<TLhs, HashSet<TRhs>> DefaultOperator;
    public override event Action? OnChange;
    private readonly Func<ObjectTable.Cell, TLhs> CellValueFunc;
    protected NTMFilter(
        Func<ObjectTable.Cell, TLhs> cellValueFunc,
        IEnumerable<NTMFilterOption<TRhs>> options,
        IEnumerable<RelOperator<TLhs, HashSet<TRhs>>> operators,
        RelOperator<TLhs, HashSet<TRhs>> defaultOperator,
        string? label = null
    )
    {
        CellValueFunc = cellValueFunc;
        _Operator = DefaultOperator = defaultOperator;
        Options = options;
        ButtonTextWhenInactive = label ?? "...";
        Operators = operators;
    }
    public override Vector2 GetSize()
    {
        var size = Text.CalcSize(ButtonText);
        size.x += ButtonPadHor * 2f;

        if (size.x < ButtonMinWidth)
        {
            size.x = ButtonMinWidth;
        }

        return size;
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;

        if (IsActive == false)
        {
            GUI.color = Globals.GUI.TextColorSecondary;
        }

        if (Widgets.Draw.ButtonTextSubtle(rect, ButtonText, GUI.color, ButtonPadHor))
        {
            Find.WindowStack.Add(OptionsWindow);
        }

        GUI.color = origGUIColor;
    }
    public override bool Eval(ObjectTable.Cell cell)
    {
        return Operator.Eval(CellValueFunc(cell), SelectedOptions);
    }
    public sealed override void Reset()
    {
        _Operator = DefaultOperator;
        Clear();
    }
    private void Clear()
    {
        SelectedOptions.Clear();
        _Info = null;
        Resize();
        OnChange?.Invoke();
    }
    private void HandleOptionClick(TRhs option)
    {
        if (SelectedOptions.Contains(option))
        {
            SelectedOptions.Remove(option);
        }
        else
        {
            if (Event.current.control == false)
            {
                SelectedOptions.Clear();
            }

            SelectedOptions.Add(option);
        }

        _Info = null;
        Resize();
        OnChange?.Invoke();
    }
    public override void NotifyChanged()
    {
        OnChange?.Invoke();
    }

    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        private readonly Widget OptionsList;
        private readonly Vector2 OptionsListSize;
        private readonly Widget Toolbar;
        private readonly Vector2 ToolbarSize;
        private static readonly Color BorderColor = Verse.Widgets.SeparatorLineColor;
        private static readonly Color BackgroundColor = Verse.Widgets.WindowBGFillColor;
        private const float OptionHoverHorShiftAmount = 4f;
        private static readonly Color OptionHoverBackgroundColor = FloatMenuOption.ColorBGActiveMouseover;
        private const float OptionPadHor = Globals.GUI.Pad;
        private const float OptionPadVer = Globals.GUI.PadXs;
        private const float OperatorButtonSize = 28f;
        private bool WillScrollHor = false;
        private Vector2 ScrollPosition;
        private static readonly float OptionWidgetHeight = Text.LineHeight + OptionPadVer * 2f;
        public OptionsWindowWidget(
            List<NTMFilterOption<TRhs>> options,
            NTMFilter<TLhs, TRhs> parent,
            IEnumerable<RelOperator<TLhs, HashSet<TRhs>>> operators
        )
        {
            doWindowBackground = false;
            drawShadow = false;
            closeOnClickedOutside = true;
            Toolbar = new VerticalContainer([
                new HorizontalContainer([
                    ..operators.Select(@operator =>
                        new Label(@operator.Symbol)
                        .TextAnchor(TextAnchor.MiddleCenter)
                        .SizeAbs(OperatorButtonSize)
                        .Color(Globals.GUI.TextColorHighlight)
                        .SkipNextExtension(() => parent.Operator != @operator)
                        .HoverShift(Globals.GUI.ButtonSubtleContentHoverOffset, -Globals.GUI.ButtonSubtleContentHoverOffset)
                        .BackgroundAtlas(Verse.Widgets.ButtonSubtleAtlas)
                        .HoverColor(GenUI.MouseoverColor)
                        .Color(Globals.GUI.TextColorSecondary)
                        .SkipNextExtension(() => parent.Operator != @operator)
                        .OnClick(() => parent.Operator = @operator)
                        .Tooltip(@operator.Description)
                    ),

                    new Label("Clear")
                    .TextAnchor(TextAnchor.MiddleLeft)
                    .PaddingAbs(Globals.GUI.PadSm, 0f)
                    .WidthIncRel(1f)
                    .HeightAbs(OperatorButtonSize)
                    .ToButtonSubtle(parent.Clear),
                ], shareFreeSpace: true)
                .WidthRel(1f),

                new Label("<i>Hold [Ctrl] to select multiple options.</i>")
                .PaddingAbs(Globals.GUI.PadSm, 0f)
                .BorderLeft(BorderColor)
                .BorderRight(BorderColor)
                .WidthRel(1f),
            ]);
            ToolbarSize = Toolbar.GetSize();

            var optionsListMaxHeight = UI.screenHeight * 0.6f - ToolbarSize.y;
            var columns = new List<List<Widget>>()
            {
                new()
            };
            var currentColumn = columns[0];
            var currentColumnHeight = 0f;

            foreach (var option in options)
            {
                Widget optionWidget = option
                .ToWidget()
                .PaddingAbs(OptionPadHor, OptionPadVer)
                .WidthRel(1f)
                .HoverShiftHor(OptionHoverHorShiftAmount)
                .Background(rect =>
                {
                    if (parent.SelectedOptions.Contains(option.Value))
                    {
                        Verse.Widgets.DrawHighlightSelected(rect);
                    }
                })
                .HoverBackground(OptionHoverBackgroundColor)
                .OnClick(() => parent.HandleOptionClick(option.Value));

                if (option.Tooltip?.Length > 0)
                {
                    optionWidget = optionWidget.Tooltip(option.Tooltip);
                }

                var optionWidgetHeight = optionWidget.GetSize().y;
                currentColumnHeight += optionWidgetHeight;

                if (currentColumnHeight > optionsListMaxHeight)
                {
                    currentColumn = [];
                    currentColumnHeight = optionWidgetHeight;
                    columns.Add(currentColumn);
                }

                if (currentColumn.Count > 0)
                {
                    optionWidget = optionWidget.PaddingAbs(0f, 0f, 1f, 0f);
                }

                currentColumn.Add(optionWidget);
            }

            var columnWidgets = new List<Widget>(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                Widget columnWidget = new VerticalContainer(column);

                if (i < columns.Count - 1)
                {
                    columnWidget = columnWidget.BorderRight(BorderColor);
                }

                columnWidgets.Add(columnWidget);
            }

            OptionsList = new HorizontalContainer(columnWidgets, stretchItems: true)
            .WidthRel(1f);
            OptionsListSize = OptionsList.GetSize();
        }
        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = Globals.GUI.Opacity;
            var origGUIColor = GUI.color;

            DoFadeEffect(rect);

            Verse.Widgets.DrawBoxSolid(rect, BackgroundColor.AdjustedForGUIOpacity());

            var rectSize = rect.size;

            Toolbar.Draw(rect.CutByY(Toolbar.GetSize().y), rectSize);

            var borderColor = BorderColor.AdjustedForGUIOpacity();
            if (Event.current.type == EventType.Repaint)
            {
                // Hor:
                // - Top
                var horRect = rect with { height = 1f };
                Verse.Widgets.DrawBoxSolid(horRect, borderColor);
                // - Bottom
                horRect.y = rect.yMax - 1f;
                Verse.Widgets.DrawBoxSolid(horRect, borderColor);
                // Ver:
                // - Left
                var verRect = rect with { width = 1f };
                Verse.Widgets.DrawBoxSolid(verRect, borderColor);
                // - Right
                verRect.x = rect.xMax - 1f;
                Verse.Widgets.DrawBoxSolid(verRect, borderColor);
            }

            rect = rect.ContractedBy(1f);

            if (WillScrollHor)
            {
                var viewRect = new Rect(0f, 0f, OptionsListSize.x, OptionsListSize.y);

                if (Event.current.type == EventType.ScrollWheel && Mouse.IsOver(rect))
                {
                    ScrollPosition.x = Mathf.Max(ScrollPosition.x + Event.current.delta.y * 20f, 0f);
                    Event.current.Use();
                }

                Verse.Widgets.BeginScrollView(rect, ref ScrollPosition, viewRect);

                OptionsList.Draw(viewRect, rectSize);

                Verse.Widgets.EndScrollView();

                Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - GenUI.ScrollBarWidth - 1f, rect.width, borderColor);
            }
            else
            {
                OptionsList.Draw(rect, rectSize);
            }

            DrawRowSeparators(rect, borderColor);

            Globals.GUI.Opacity = origGUIOpacity;
            GUI.color = origGUIColor;
        }
        private void DoFadeEffect(Rect rect)
        {
            rect = rect.ContractedBy(-5f);

            const float maxAllovedMouseDistFromRect = 95f;

            if (rect.Contains(Event.current.mousePosition) == false)
            {
                var mouseDistFromRect = GenUI.DistFromRect(rect, Event.current.mousePosition);

                Globals.GUI.Opacity = 1f - mouseDistFromRect / maxAllovedMouseDistFromRect;
                GUI.color = GUI.color.AdjustedForGUIOpacity();

                if (mouseDistFromRect > maxAllovedMouseDistFromRect)
                {
                    Close();
                }
            }
        }
        public override void Close(bool doCloseSound = false)
        {
            SoundDefOf.FloatMenu_Cancel.PlayOneShotOnCamera();
            base.Close(doCloseSound);
        }
        protected override void SetInitialSizeAndPosition()
        {
            var position = UI.MousePositionOnUIInverted;
            var size = new Vector2(
                Mathf.Max(ToolbarSize.x, OptionsListSize.x + 2f),
                ToolbarSize.y + OptionsListSize.y + 2f
            );
            var maxWidth = UI.screenWidth * 0.9f;

            if (size.x > maxWidth)
            {
                WillScrollHor = true;
                size.x = maxWidth;
                size.y += GenUI.ScrollBarWidth + 1f;
            }

            if (position.x + size.x > UI.screenWidth)
            {
                position.x = UI.screenWidth - size.x - Globals.GUI.Pad;
            }

            if (position.y + size.y > UI.screenHeight)
            {
                position.y = UI.screenHeight - size.y - Globals.GUI.Pad;
            }

            windowRect = new Rect(position, size);
        }
        private static void DrawRowSeparators(Rect rect, Color color)
        {
            var y = rect.y + OptionWidgetHeight;

            if (Event.current.type != EventType.Repaint) return;

            while (y < rect.yMax)
            {
                Verse.Widgets.DrawLineHorizontal(rect.x, y, rect.width, color);
                y += OptionWidgetHeight + 1f;
            }
        }
    }
}

public readonly record struct NTMFilterOption<TValue>
{
    public TValue Value { get; }
    public string Label { get; }
    public Widget? Icon { get; }
    public string? Tooltip { get; }
    public NTMFilterOption()
    {
        Value = default;
        Label = "<i>Undefined</i>";
    }
    public NTMFilterOption(TValue value, string label, Widget? icon = null, string? tooltip = null)
    {
        Value = value;
        Label = label;
        Icon = icon;
        Tooltip = tooltip;
    }
    public Widget ToWidget()
    {
        var label = new Label(Label);

        if (Icon != null)
        {
            return new HorizontalContainer([Icon, label], Globals.GUI.PadSm);
        }

        return label;
    }
}
