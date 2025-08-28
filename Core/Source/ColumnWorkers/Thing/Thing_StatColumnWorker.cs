using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public class Thing_StatColumnWorker : ColumnWorker<ThingAlike>
{
    public static readonly Regex NumberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;
    protected StatDef Stat { get; }
    public Thing_StatColumnWorker(StatColumnDef columnDef) : base(columnDef, CellStyleType.Number)
    {
        Stat = columnDef.stat;
    }
    protected virtual string? GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest statRequest)
    {
        return stat.Worker.GetStatDrawEntryLabel(stat, value, _ToStringNumberSense, statRequest);
    }
    public override ObjectTable.Cell GetCell(ThingAlike thing)
    {
        return new Cell(thing, this);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> _)
    {
        yield return new(ColumnDef.Title, new NumberFilter<Cell>(cell => cell.ValueDisplayed, this));
    }

    private class Cell : ObjectTable.WidgetCell
    {
        public override event Action? OnChange;
        protected override Widget? Widget { get; set; }
        public float ValueRaw { get; private set; }
        public decimal ValueDisplayed { get; private set; }
        private readonly bool IsValidForThing;
        private readonly Thing_StatColumnWorker Column;
        private readonly StatDef Stat;
        private readonly StatWorker StatWorker;
        private readonly ThingAlike Thing;
        public Cell(ThingAlike thing, Thing_StatColumnWorker column)
        {
            Thing = thing;
            Column = column;
            Stat = column.Stat;
            StatWorker = column.Stat.Worker;
            var statRequest = thing.Thing != null
                ? StatRequest.For(thing.Thing)
                : StatRequest.For(thing.Def, thing.StuffDef);
            IsValidForThing = StatWorker.ShouldShowFor(statRequest);

            if (IsValidForThing)
            {
                var statValue = StatWorker.GetValue(statRequest);

                Refresh(statRequest, statValue);
            }

            if (thing.Thing != null && column.Stat.immutable == false && IsValidForThing)
            {
                column.OnRefresh += HandleColumnRefresh;
            }
        }
        private void Refresh(StatRequest statRequest, float statValue)
        {
            if (IsValidForThing && statValue != 0f)
            {
                var cellText = Column.GetStatDrawEntryLabel(Stat, statValue, _ToStringNumberSense, statRequest);

                if (cellText != null)
                {
                    var displayedValue = 0m;
                    var match = NumberRegex.Match(cellText);

                    if (match.Success)
                    {
                        displayedValue = decimal.Parse(match.Groups[1].Captures[0].Value);
                    }

                    var tooltip = StatWorker.GetExplanationFull(statRequest, _ToStringNumberSense, statValue);

                    Widget widget = new Label(cellText)
                    .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

                    if (tooltip != null)
                    {
                        widget = widget.Tooltip(tooltip);
                    }

                    Widget = widget;
                    ValueRaw = statValue;
                    ValueDisplayed = displayedValue;
                }
            }
            else
            {
                Widget = null;
                ValueRaw = 0f;
                ValueDisplayed = 0m;
            }

            OnChange?.Invoke();
        }
        private void HandleColumnRefresh()
        {
            if (IsValidForThing)
            {
                var statRequest = Thing.Thing != null
                    ? StatRequest.For(Thing.Thing)
                    : StatRequest.For(Thing.Def, Thing.StuffDef);
                var statValue = StatWorker.GetValue(statRequest);

                if (ValueRaw != statValue)
                {
                    Refresh(statRequest, statValue);
                }
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return ValueDisplayed.CompareTo(((Cell)cell).ValueDisplayed);
        }
        public override void Dispose()
        {
            Column.OnRefresh -= HandleColumnRefresh;
        }
    }
}
