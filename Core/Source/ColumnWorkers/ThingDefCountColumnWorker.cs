using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefCountColumnWorker<TObject> : ColumnWorker<TObject, (ThingDef? Def, decimal Count)>
{
    protected ThingDefCountColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.Number)
    {
    }
    protected abstract (ThingDef? Def, decimal Count) GetValue(TObject @object);
    protected sealed override Cell GetCell(TObject @object)
    {
        var thingDefCount = GetValue(@object);
        var (thingDef, count) = thingDefCount;

        if (thingDef != null && count > 0)
        {
            var widget = new HorizontalContainer([
                new Label(count.ToString()).PaddingRel(1f, 0f, 0f, 0f),
                new ThingIcon(thingDef)
                .ToButtonGhostly(() => Draw.DefInfoDialog(thingDef))
                .Tooltip(thingDef.LabelCap),
            ], Globals.GUI.PadSm, true)
            .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, thingDefCount);
        }

        return new(null, (null, 0m));
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords)
    {
        var countFilter = Make.NumberFilter<TObject>(@object => Cells[@object].Data.Count);
        var typeFilter = Make.OTMThingDefFilter(@object => Cells[@object].Data.Def, tableRecords);

        yield return new(new Label("Amount"), countFilter);
        yield return new(new Label("Type"), typeFilter);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        var count1 = Cells[object1].Data.Count;
        var count2 = Cells[object2].Data.Count;

        return Comparer<decimal?>.Default.Compare(count1, count2);
    }
}
