using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class LifeExpectancyColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        RaceProperties? raceProps = thingDef.race;

        if (raceProps != null)
        {
            return new NumberCell(raceProps.lifeExpectancy.ToDecimal(0), "0 y");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
