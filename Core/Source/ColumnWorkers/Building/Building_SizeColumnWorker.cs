using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Building_SizeColumnWorker : ColumnWorker<ThingAlike, IntVec2>
{
    public Building_SizeColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.Number)
    {
    }
    protected override Cell GetCell(ThingAlike thing)
    {
        var size = thing.Def.size;
        // Because 4x5=5x4.
        size = new IntVec2(Math.Max(size.x, size.z), Math.Min(size.x, size.z));
        var widget = new Label(size.ToStringCross()).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

        return new(widget, size);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        var filterOptions = tableRecords
            .Select(record => Cells[record].Data)
            .Distinct()
            .OrderBy(size => size.Area)
            .Select(size => new NTMFilterOption<IntVec2>(size, size.ToStringCross()));

        yield return new(ColumnDef.Title, Make.OTMFilter<ThingAlike, IntVec2>(@object => Cells[@object].Data, filterOptions));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.size.Area.CompareTo(thing2.Def.size.Area);
    }
}
