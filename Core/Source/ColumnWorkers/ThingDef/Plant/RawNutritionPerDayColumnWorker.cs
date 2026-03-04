using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class RawNutritionPerDayColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps is { growDays: > 0f })
            {
                float nutrition = thingDef.GetStatValuePerceived(StatDefOf.Nutrition);
                decimal cellValue = (nutrition / plantProps.GetGrowDaysActual()).ToDecimal(3);

                return new NumberTableCell(cellValue, "0.000/d");
            }
        }

        return default;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
