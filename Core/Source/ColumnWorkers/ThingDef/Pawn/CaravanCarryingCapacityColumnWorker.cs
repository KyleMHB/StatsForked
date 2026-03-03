using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class CaravanCarryingCapacityColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        RaceProperties? raceProps = thingDef.race;

        if (raceProps != null)
        {
            decimal cellValue = (raceProps.baseBodySize * MassUtility.MassCapacityPerBodySize).ToDecimal(0);

            return new NumberCell.Constant(cellValue, "0 kg");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
