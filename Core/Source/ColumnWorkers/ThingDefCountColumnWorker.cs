using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefCountColumnWorker<TObject> : ColumnWorker<TObject>
{
    protected ThingDefCountColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.Number)
    {
    }
    protected abstract (ThingDef? Def, decimal Count) GetValue(TObject @object);
    public sealed override ObjectTable.Cell GetCell(TObject @object)
    {
        var thingDefCount = GetValue(@object);
        var (thingDef, count) = thingDefCount;

        return new Cell(thingDef, count);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> contextObjects)
    {
        var countFilter = new NumberFilter<Cell>(cell => cell.Count, this);
        var typeFilterOptions = contextObjects
        .Select(@object => GetValue(@object).Def)
        .Distinct()
        .OrderBy(thingDef => thingDef?.label)
        .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
            thingDef => thingDef == null
                ? new()
                : new(thingDef, thingDef.LabelCap, new ThingIcon(thingDef))
        );
        var typeFilter = new OTMFilter<Cell, ThingDef?>(cell => cell.ThingDef, typeFilterOptions, this);

        yield return new(new Label("Amount"), countFilter);
        yield return new(new Label("Type"), typeFilter);
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public ThingDef? ThingDef { get; }
        public decimal Count { get; }
        public Cell(ThingDef? thingDef, decimal count)
        {
            ThingDef = thingDef;
            Count = count;

            if (thingDef != null && count > 0)
            {
                Widget = new HorizontalContainer([
                    new Label(count.ToString()).PaddingRel(1f, 0f, 0f, 0f),
                    new ThingIcon(thingDef)
                    .ToButtonGhostly(() => Widgets.Draw.DefInfoDialog(thingDef))
                    .Tooltip(thingDef.LabelCap),
                ], Globals.GUI.PadSm, true)
                .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Count.CompareTo(((Cell)cell).Count);
        }
    }
}
