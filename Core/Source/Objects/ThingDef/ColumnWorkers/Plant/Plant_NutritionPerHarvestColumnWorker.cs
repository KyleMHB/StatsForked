using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_NutritionPerHarvestColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_NutritionPerHarvestColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
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
