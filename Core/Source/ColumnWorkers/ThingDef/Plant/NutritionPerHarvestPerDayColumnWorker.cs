using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class NutritionPerHarvestPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        // TODO: This is mostly copy paste from NutritionPerHarvestColumnWorker.
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps is { harvestedThingDef: not null, growDays: > 0f })
        {
            float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            float nutritionPerHarvest = plantProps.harvestYield * productNutrition;
            decimal cellValue = (nutritionPerHarvest / plantProps.GetGrowDaysActual()).ToDecimal(3);

            return new NumberCell.Constant(cellValue, "0.000/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
