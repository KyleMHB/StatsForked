using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class LifeExpectancyColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        RaceProperties? raceProps = thingDef.race;

        if (raceProps != null)
        {
            return new NumberCell.Constant(raceProps.lifeExpectancy.ToDecimal(0), "0 y");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
