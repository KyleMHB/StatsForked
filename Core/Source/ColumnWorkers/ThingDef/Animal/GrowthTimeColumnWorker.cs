using RimWorld;
using Stats.Extensions;
using Verse;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class GrowthTimeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                decimal cellValue = AnimalProductionUtility.DaysToAdulthood(thingDef).ToDecimal(0);

                return new NumberTableCell(cellValue, "0 d");
            }
        }

        return default;
    }
}
