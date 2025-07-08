using RimWorld;

namespace Stats;

public sealed class Plant_NutritionPerHarvestPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_NutritionPerHarvestPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.000/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        // TODO: This is mostly copy paste from Plant_NutritionPerHarvestColumnWorker.
        var plantProps = thing.Def.plant;

        if (plantProps is { harvestedThingDef: not null, growDays: > 0f })
        {
            var productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var nutritionPerHarvest = plantProps.harvestYield * productNutrition;

            return (nutritionPerHarvest / plantProps.growDays).ToDecimal(3);
        }

        return 0m;
    }
}
