using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats.Widgets;

internal abstract class NTMFilter<TObject, TObjectValue, TOption> : FilterWidget<TObject>
{
    public override bool IsActive => SelectedOptions.Count > 0;
    // TODO: See if IEnumerable is most fitting type here.
    private readonly IEnumerable<NTMFilterOption<TOption>> Options;
    private List<NTMFilterOption<TOption>>? _OptionsList;
    private List<NTMFilterOption<TOption>> OptionsList => _OptionsList ??= Options.ToList();
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
    private readonly IEnumerable<RelOperator<TObjectValue, HashSet<TOption>>> Operators;
    private string ButtonText => IsActive ? Info : ButtonTextWhenInactive;
    private readonly string ButtonTextWhenInactive;
    private const float ButtonMinWidth = 24f;
    private const float ButtonPadHor = Globals.GUI.PadSm;
    private readonly HashSet<TOption> SelectedOptions = [];
    private RelOperator<TObjectValue, HashSet<TOption>> _Operator;
    protected RelOperator<TObjectValue, HashSet<TOption>> Operator
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
            OnChange?.Invoke(this);
        }
    }
    private readonly RelOperator<TObjectValue, HashSet<TOption>> DefaultOperator;
    public override event Action<FilterWidget<TObject>>? OnChange;
    private readonly Func<TObject, TObjectValue> ObjectValueFunc;
    protected NTMFilter(
        Func<TObject, TObjectValue> objectValueFunc,
        IEnumerable<NTMFilterOption<TOption>> options,
        IEnumerable<RelOperator<TObjectValue, HashSet<TOption>>> operators,
        RelOperator<TObjectValue, HashSet<TOption>> defaultOperator,
        string? label = null
    )
    {
        ObjectValueFunc = objectValueFunc;
        _Operator = DefaultOperator = defaultOperator;
        Options = options;
        ButtonTextWhenInactive = label ?? "...";
        Operators = operators;
    }
    protected override Vector2 CalcSize()
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
    public override bool Eval(TObject @object)
    {
        return Operator.Eval(ObjectValueFunc(@object), SelectedOptions);
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
        OnChange?.Invoke(this);
    }
    private void HandleOptionClick(TOption option)
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
        OnChange?.Invoke(this);
    }

    private sealed class OptionsWindowWidget : Window
    {
        protected override float Margin => 0f;
        public override Vector2 InitialSize => Widget.GetSize();
        private readonly Widget Widget;
        private static readonly Color BorderColor = Verse.Widgets.SeparatorLineColor;
        private static readonly Color BackgroundColor = Verse.Widgets.WindowBGFillColor;
        private const float OptionHoverHorShiftAmount = 4f;
        private static readonly Color OptionHoverBackgroundColor = FloatMenuOption.ColorBGActiveMouseover;
        private const float OptionPadHor = Globals.GUI.Pad;
        private const float OptionPadVer = Globals.GUI.PadXs;
        private const int ColumnCapacity = 15;
        public OptionsWindowWidget(
            List<NTMFilterOption<TOption>> options,
            NTMFilter<TObject, TObjectValue, TOption> parent,
            IEnumerable<RelOperator<TObjectValue, HashSet<TOption>>> operators
        )
        {
            doWindowBackground = false;
            drawShadow = false;
            closeOnClickedOutside = true;

            var columns = new List<List<Widget>>()
            {
                new(ColumnCapacity)
            };
            var currentColumn = columns[0];

            foreach (var option in options)
            {
                if (currentColumn.Count - ColumnCapacity == 0)
                {
                    columns.Add(currentColumn = new(ColumnCapacity));
                }

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
                .OnClick(() => parent.HandleOptionClick(option.Value))
                .BorderBottom(BorderColor);

                if (option.Tooltip?.Length > 0)
                {
                    optionWidget = optionWidget.Tooltip(option.Tooltip);
                }

                currentColumn.Add(optionWidget);
            }
            var columnWidgets = new List<Widget>(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                var column = columns[i];
                Widget columnWidget = new VerticalContainer(column)
                .Background(BackgroundColor)
                .BorderRight(BorderColor);

                if (i == 0)
                {
                    columnWidget = columnWidget.BorderLeft(BorderColor);
                }

                columnWidgets.Add(columnWidget);
            }

            Widget = new VerticalContainer([
                new HorizontalContainer([
                    ..operators.Select(@operator =>
                        new Label(@operator.Symbol)
                        .Color(Globals.GUI.TextColorHighlight)
                        .SkipNextExtension(() => parent.Operator != @operator)
                        .PaddingAbs(Globals.GUI.PadSm, Globals.GUI.PadXs)
                        .HoverShift(Globals.GUI.ButtonSubtleContentHoverOffset, -Globals.GUI.ButtonSubtleContentHoverOffset)
                        .BackgroundAtlas(Verse.Widgets.ButtonSubtleAtlas)
                        .HoverColor(GenUI.MouseoverColor)
                        .Color(Globals.GUI.TextColorSecondary)
                        .SkipNextExtension(() => parent.Operator != @operator)
                        .OnClick(() => parent.Operator = @operator)
                        .Tooltip(@operator.Description)
                    ),

                    new Label("Clear")
                    .PaddingAbs(Globals.GUI.PadSm, Globals.GUI.PadXs)
                    .WidthIncRel(1f)
                    .ToButtonSubtle(parent.Clear),
                ], shareFreeSpace: true)
                .WidthRel(1f),

                new Label("<i>Hold [Ctrl] to select multiple options.</i>")
                .PaddingAbs(Globals.GUI.PadSm, 0f)
                .Background(BackgroundColor)
                .BorderLeft(BorderColor).BorderRight(BorderColor).BorderBottom(BorderColor)
                .WidthRel(1f),

                new HorizontalContainer(columnWidgets, stretchItems: true)
                .WidthRel(1f),
            ]);
        }
        public override void DoWindowContents(Rect rect)
        {
            var origGUIOpacity = Globals.GUI.Opacity;
            var origGUIColor = GUI.color;

            DoFadeEffect(rect);

            Widget.Draw(rect, rect.size);

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
            var size = InitialSize;

            if (position.x + size.x > UI.screenWidth)
            {
                position.x = UI.screenWidth - size.x;
            }

            if (position.y + size.y > UI.screenHeight)
            {
                position.y = UI.screenHeight - size.y;
            }

            windowRect = new Rect(position, size);
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
