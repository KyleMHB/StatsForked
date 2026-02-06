using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable;
using Stats.Widgets;
using Verse;

namespace Stats;

public class StatColumnWorker : ColumnWorker, IColumnWorker<VirtualThing>, IColumnWorker<Thing>
{
    public static readonly Regex NumberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;
    protected StatDef Stat { get; }
    public StatColumnWorker(StatColumnDef columnDef) : base(columnDef, IColumnWorker.CellStyleType.Number)
    {
        Stat = columnDef.stat;
    }
    protected virtual string? GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest statRequest)
    {
        return stat.Worker.GetStatDrawEntryLabel(stat, value, _ToStringNumberSense, statRequest);
    }
    public ObjectTableWidget.Cell GetCell(VirtualThing thing)
    {
        return new Cell(thing, this);
    }
    public IEnumerable<ObjectTableWidget.ColumnPart> GetObjectProps(TableWorker<VirtualThing> _)
    {
        yield return new(Def.Title, new NumberFilter(cell => ((Cell)cell).ValueDisplayed));
    }
    public ObjectTableWidget.Cell GetCell(Thing thing)
    {
        throw new NotImplementedException();
    }
    public IEnumerable<ObjectTableWidget.ColumnPart> GetObjectProps(TableWorker<Thing> _)
    {
        throw new NotImplementedException();
    }

    private class Cell : ObjectTableWidget.WidgetCell, ObjectTableWidget.Cell.IRefreshable
    {
        protected override Widget? Widget { get; set; }
        public float ValueRaw { get; private set; }
        public decimal ValueDisplayed { get; private set; }
        private readonly bool IsValidForThing;
        private readonly StatColumnWorker Column;
        private readonly StatDef Stat;
        private readonly StatWorker StatWorker;
        private readonly VirtualThing Thing;
        public Cell(VirtualThing thing, StatColumnWorker column)
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

                Refresh_Int(statRequest, statValue);
            }

            //if (thing.Thing != null && column.Stat.immutable == false && IsValidForThing)
            //{
            //}
        }
        private void Refresh_Int(StatRequest statRequest, float statValue)
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
                    .PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);

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
        }
        public bool Refresh()
        {
            if (IsValidForThing)
            {
                var statRequest = Thing.Thing != null
                    ? StatRequest.For(Thing.Thing)
                    : StatRequest.For(Thing.Def, Thing.StuffDef);
                var statValue = StatWorker.GetValue(statRequest);

                if (ValueRaw != statValue)
                {
                    Refresh_Int(statRequest, statValue);

                    return true;
                }
            }

            return false;
        }
        public override int CompareTo(ObjectTableWidget.Cell cell)
        {
            return ValueDisplayed.CompareTo(((Cell)cell).ValueDisplayed);
        }
    }
}
