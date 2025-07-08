using RimWorld;

namespace Stats;

public sealed class Animal_MilkPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_MilkPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps is { milkIntervalDays: > 0 })
        {
            return ((float)milkableCompProps.milkAmount / milkableCompProps.milkIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
