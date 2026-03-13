using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class NutritionPerHarvestPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            // TODO: This is mostly copy paste from NutritionPerHarvestColumnWorker.
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps is { harvestedThingDef: not null, growDays: > 0f })
            {
                float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float nutritionPerHarvest = plantProps.harvestYield * productNutrition;
                decimal cellValue = (nutritionPerHarvest / plantProps.GetGrowDaysActual()).ToDecimal(3);

                return new NumberTableCell(cellValue, "0.000/d");
            }
        }

        return default;
    }
}
