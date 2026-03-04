using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class NutritionPerHarvestColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.harvestedThingDef != null)
            {
                float productNutrition = plantProps.harvestedThingDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float nutritionPerHarvest = plantProps.harvestYield * productNutrition;

                return new NumberTableCell(nutritionPerHarvest.ToDecimal(2), "0.00");
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
