using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers;

public sealed class DimensionsColumnWorker : ColumnWorker<VirtualThing>
{
    public DimensionsColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.Number, TODO)
    {
    }
    public override ObjectTableWidget.Cell GetCell(VirtualThing thing)
    {
        return new Cell(thing, this);
    }
    private IntVec2 GetSize(VirtualThing thing)
    {
        var size = thing.Def.size;

        // Because 4x5=5x4.
        return new IntVec2(Math.Max(size.x, size.z), Math.Min(size.x, size.z));
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<VirtualThing> contextObjects)
    {
        var filterOptions = contextObjects
            .Select(GetSize)
            .Distinct()
            .OrderBy(size => size.Area)
            .Select(size => new NTMFilterOption<IntVec2>(size, size.ToStringCross()));

        yield return new(Def.Title, new OTMFilter<Cell, IntVec2>(cell => cell.BuildingSize, filterOptions, this));
    }

    private sealed class Cell : ObjectTableWidget.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public IntVec2 BuildingSize { get; }
        public int BuildingArea { get; }
        public Cell(VirtualThing thing, DimensionsColumnWorker column)
        {
            var size = column.GetSize(thing);

            BuildingSize = size;
            Widget = new Label(size.ToStringCross()).PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
            BuildingArea = size.Area;
        }
        public override int CompareTo(ObjectTableWidget.Cell cell)
        {
            return BuildingArea.CompareTo(((Cell)cell).BuildingArea);
        }
    }
}
