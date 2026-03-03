using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class RawNutritionPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps is { growDays: > 0f })
        {
            float nutrition = thingDef.GetStatValuePerceived(StatDefOf.Nutrition);
            decimal cellValue = (nutrition / plantProps.GetGrowDaysActual()).ToDecimal(3);

            return new NumberCell.Constant(cellValue, "0.000/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
