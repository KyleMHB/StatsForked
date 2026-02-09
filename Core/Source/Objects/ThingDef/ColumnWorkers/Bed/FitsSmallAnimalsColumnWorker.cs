using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Bed;

public sealed class FitsSmallAnimalsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        BuildingProperties? buildingProps = thingDef.building;

        if (buildingProps != null)
        {
            return new BooleanCell(buildingProps.bed_humanlike == false);
        }

        return BooleanCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
