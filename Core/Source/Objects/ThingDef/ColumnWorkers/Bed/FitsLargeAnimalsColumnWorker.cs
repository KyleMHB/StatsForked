using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Bed;

public sealed class FitsLargeAnimalsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        BuildingProperties? buildingProps = thingDef.building;

        if (buildingProps != null)
        {
            bool cellValue = buildingProps.bed_humanlike == false && buildingProps.bed_maxBodySize > 0.55f;

            return new BooleanCell(cellValue);
        }

        return BooleanCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => BooleanCell.GetDescriptor(columnDef);
}
