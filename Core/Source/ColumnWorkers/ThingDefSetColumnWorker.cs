using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefSetColumnWorker<TObject, TDef> : ColumnWorker<TObject> where TDef : ThingDef
{
    protected ThingDefSetColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    protected abstract HashSet<TDef> GetValue(TObject @object);
    public sealed override ObjectTable.Cell GetCell(TObject @object)
    {
        var defs = GetValue(@object);

        return new Cell(defs);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> contextObjects)
    {
        var options = contextObjects
            .SelectMany(GetValue)
            .Distinct()
            .OrderBy(thingDef => thingDef.label)
            .Select<TDef, NTMFilterOption<TDef>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
            );

        yield return new(Def.Title, new MTMFilter<Cell, TDef>(cell => cell.Defs, options, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public HashSet<TDef> Defs { get; }
        public Cell(HashSet<TDef> defs)
        {
            Defs = defs;

            if (defs.Count > 0)
            {
                var icons = new List<Widget>(defs.Count);

                foreach (var thingDef in defs.OrderBy(thingDef => thingDef.label))
                {
                    var icon = new ThingDefIcon(thingDef)
                    .ToButtonGhostly(() => Widgets.Draw.DefInfoDialog(thingDef))
                    .Tooltip(thingDef.LabelCap);

                    icons.Add(icon);
                }

                Widget = new HorizontalContainer(icons, Globals.GUI.PadSm).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Defs.Count.CompareTo(((Cell)cell).Defs.Count);
        }
    }
}
