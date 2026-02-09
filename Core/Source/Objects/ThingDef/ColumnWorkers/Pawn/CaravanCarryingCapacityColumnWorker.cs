using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class CaravanCarryingCapacityColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        RaceProperties? raceProps = thingDef.race;

        if (raceProps != null)
        {
            decimal cellValue = (raceProps.baseBodySize * MassUtility.MassCapacityPerBodySize).ToDecimal(0);

            return new NumberCell(cellValue, "0 kg");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
