using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class NutritionPerHarvestColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.harvestedThingDef != null)
            {
                float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float nutritionPerHarvest = plantProps.harvestYield * productNutrition;

                return new NumberCell(nutritionPerHarvest.ToDecimal(2), "0.00");
            }
        }

        return default;
    }
}
