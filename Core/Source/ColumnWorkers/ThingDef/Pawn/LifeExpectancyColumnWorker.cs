using Stats.Extensions;
using Verse;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class LifeExpectancyColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                return new NumberTableCell(raceProps.lifeExpectancy.ToDecimal(0), "0 y");
            }
        }

        return default;
    }
}
