using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefSetColumnWorker<TObject, TDef> : ColumnWorker<TObject, HashSet<TDef>> where TDef : ThingDef
{
    protected ThingDefSetColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected abstract HashSet<TDef> GetValue(TObject @object);
    protected sealed override Cell GetCell(TObject @object)
    {
        var defs = GetValue(@object);

        if (defs.Count > 0)
        {
            var icons = new List<Widget>(defs.Count);

            foreach (var thingDef in defs.OrderBy(thingDef => thingDef.label))
            {
                var icon = new ThingIcon(thingDef)
                .ToButtonGhostly(() => Draw.DefInfoDialog(thingDef))
                .Tooltip(thingDef.LabelCap);

                icons.Add(icon);
            }

            var widget = new HorizontalContainer(icons, Globals.GUI.PadSm).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, defs);
        }

        return new(null, []);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.MTMThingDefFilter(@object => Cells[@object].Data, tableRecords));
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return Cells[object1].Data.Count.CompareTo(Cells[object2].Data.Count);
    }
}
