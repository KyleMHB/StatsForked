using RimWorld;

namespace Stats;

public sealed class Animal_WoolPerDayColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_WoolPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps is { shearIntervalDays: > 0 })
        {
            return ((float)shearableCompProps.woolAmount / shearableCompProps.shearIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
