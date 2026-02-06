using RimWorld;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Milkable;

public sealed class MilkNutritionPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public MilkNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkDef: not null, milkIntervalDays: > 0 })
        {
            var milkNutrition = milkableCompProps.milkDef.GetStatValuePerceived(StatDefOf.Nutrition);
            var milkPerDay = (float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays;

            return (milkPerDay * milkNutrition).ToDecimal(2);
        }

        return 0m;
    }
}
