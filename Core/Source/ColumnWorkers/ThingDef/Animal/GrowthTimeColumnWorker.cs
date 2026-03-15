using RimWorld;
using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class GrowthTimeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                decimal cellValue = AnimalProductionUtility.DaysToAdulthood(thingDef).ToDecimal(0);

                return new NumberCell(cellValue, "0 d");
            }
        }

        return default;
    }
}
