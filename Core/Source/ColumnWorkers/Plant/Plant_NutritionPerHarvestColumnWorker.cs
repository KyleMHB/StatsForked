using RimWorld;

namespace Stats;

public sealed class Plant_NutritionPerHarvestColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Plant_NutritionPerHarvestColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.harvestedThingDef != null)
        {
            var productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var nutritionPerHarvest = plantProps.harvestYield * productNutrition;

            return nutritionPerHarvest.ToDecimal(2);
        }

        return 0m;
    }
}
