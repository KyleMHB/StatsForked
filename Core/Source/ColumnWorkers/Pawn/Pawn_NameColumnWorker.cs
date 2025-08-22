using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Pawn_NameColumnWorker : ColumnWorker<ThingAlike, string>
{
    public Pawn_NameColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected override Cell GetCell(ThingAlike thing)
    {
        if (thing.Thing is Pawn pawn)
        {
            var text = pawn.Name.ToStringShort.CapitalizeFirst() ?? "";
            var widget = new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, text);
        }

        return new(null, "");
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.StringFilter<ThingAlike>(thing => Cells[thing].Data));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Comparer<string?>.Default.Compare(Cells[thing1].Data, Cells[thing2].Data);
    }
    public override bool Refresh()
    {
        // TODO: Handle name change.
        return base.Refresh();
    }
}
