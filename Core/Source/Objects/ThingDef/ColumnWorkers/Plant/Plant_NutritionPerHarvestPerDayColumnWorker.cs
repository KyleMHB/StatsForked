using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_NutritionPerHarvestPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_NutritionPerHarvestPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.000/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        // TODO: This is mostly copy paste from Plant_NutritionPerHarvestColumnWorker.
        var plantProps = thing.Def.plant;

        if (plantProps is { harvestedThingDef: not null, growDays: > 0f })
        {
            var productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var nutritionPerHarvest = plantProps.harvestYield * productNutrition;

            return (nutritionPerHarvest / plantProps.GetGrowDaysActual()).ToDecimal(3);
        }

        return 0m;
    }
}
