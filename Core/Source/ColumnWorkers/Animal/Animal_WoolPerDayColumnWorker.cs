using RimWorld;

namespace Stats;

public sealed class Animal_WoolPerDayColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_WoolPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return ((float)shearableCompProps.woolAmount / shearableCompProps.shearIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
