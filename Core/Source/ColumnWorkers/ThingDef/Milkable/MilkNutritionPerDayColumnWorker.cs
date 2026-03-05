using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkNutritionPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

            if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
            {
                float milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;
                decimal cellValue = (milkPerDay * milkNutrition).ToDecimal(2);

                return new NumberTableCell(cellValue, "0.00/d");
            }
        }

        return default;
    }
}
