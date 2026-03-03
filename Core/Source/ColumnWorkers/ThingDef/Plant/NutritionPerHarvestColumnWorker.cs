using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class NutritionPerHarvestColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps?.harvestedThingDef != null)
        {
            float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float nutritionPerHarvest = plantProps.harvestYield * productNutrition;

            return new NumberCell.Constant(nutritionPerHarvest.ToDecimal(2), "0.00");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
