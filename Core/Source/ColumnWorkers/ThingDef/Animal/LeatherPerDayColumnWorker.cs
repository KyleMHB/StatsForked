using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class LeatherPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

                if (growthTime > 0f)
                {
                    float leatherAmount = thingDef.GetStatValuePerceived(StatDefOf.LeatherAmount);
                    decimal cellValue = (leatherAmount / growthTime).ToDecimal(1);

                    return new NumberTableCell(cellValue, "0.0/d");
                }
            }
        }

        return default;
    }
}
