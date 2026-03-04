using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class NutritionPerHarvestPerDayColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
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
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
