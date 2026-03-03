using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ObjectTable;
using Stats.TableCells;
using Stats.TableWorkers;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class TechLevelColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    //public TechLevelColumnWorker : base(columnDef, CellStyleType.String, TODO)
    //{
    //}
    public override ObjectTableWidget.Cell GetCell(VirtualThing thing)
    {
        return new Cell(thing.Def.techLevel);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<VirtualThing> contextObjects)
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
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        TechLevel techLevel = thingDef.techLevel;

        if (techLevel != TechLevel.Undefined)
        {

        }

        throw new NotImplementedException();
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        throw new NotImplementedException();
    }

    private sealed class TechLevelCell : Cell
    {
        private readonly TechLevel Value;
        private readonly string Text;
        private readonly Vector2 Size;
        public TechLevelCell(TechLevel techLevel)
        {
            Value = techLevel;
            Text = techLevel.ToStringHuman().CapitalizeFirst();
        }
        public override void Draw(Rect rect, Vector2 containerSize)
        {
            throw new NotImplementedException();
        }
        public override Vector2 GetSize()
        {
            throw new NotImplementedException();
        }
        public override void Refresh()
        {
        }
        static private TechLevel GetValue(Cell cell)
        {
            return ((TechLevelCell)cell).Value;
        }
        static private int Compare(Cell cell1, Cell cell2)
        {
            return GetValue(cell1).CompareTo(GetValue(cell2));
        }
    }
}
