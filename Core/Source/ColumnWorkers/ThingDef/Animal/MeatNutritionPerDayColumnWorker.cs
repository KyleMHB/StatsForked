using RimWorld;
using Stats.TableCells;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class MeatNutritionPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps is { meatDef: not null })
            {
                float growthTime = AnimalProductionUtility.DaysToAdulthood(thingDef);

                if (growthTime > 0f)
                {
                    float meatNutrition = thingDef.race.meatDef.GetStatValuePerceived(StatDefOf.Nutrition);
                    float meatAmount = AnimalProductionUtility.AdultMeatAmount(thingDef);
                    float meatPerDay = meatAmount / growthTime;
                    decimal cellValue = (meatPerDay * meatNutrition).ToDecimal(2);

                    return new NumberTableCell(cellValue, "0.00/d");
                }
            }
        }

        return default;
    }
}
