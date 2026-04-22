using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Milkable;

public sealed class MilkNutritionPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();

            if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
            {
                float milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;
                decimal cellValue = (milkPerDay * milkNutrition).ToDecimal(2);

                return new NumberCell(cellValue, "0.00/d");
            }
        }

        return default;
    }
}
