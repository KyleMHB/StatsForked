using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Thing_TechLevelColumnWorker : ColumnWorker<AbstractThing>
{
    public Thing_TechLevelColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    public override ObjectTable.Cell GetCell(AbstractThing thing)
    {
        return new Cell(thing.Def.techLevel);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<AbstractThing> contextObjects)
    {
        var options = contextObjects
            .Select(thing => thing.Def.techLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel)
            .Select<TechLevel, NTMFilterOption<TechLevel>>(
                techLevel => new(techLevel, techLevel.ToStringHuman().CapitalizeFirst())
            );

        yield return new(Def.Title, new OTMFilter<Cell, TechLevel>(cell => cell.Value, options, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public TechLevel Value { get; }
        public Cell(TechLevel value)
        {
            Value = value;

            if (value != TechLevel.Undefined)
            {
                var text = value.ToStringHuman().CapitalizeFirst();
                var widget = new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

                Widget = widget;
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Value.CompareTo(((Cell)cell).Value);
        }
    }
}
