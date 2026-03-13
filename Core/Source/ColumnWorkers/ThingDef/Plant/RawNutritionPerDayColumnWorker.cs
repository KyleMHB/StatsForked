using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class RawNutritionPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
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
}
