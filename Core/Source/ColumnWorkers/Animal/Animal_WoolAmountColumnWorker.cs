using RimWorld;

namespace Stats;

public sealed class Animal_WoolAmountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_WoolAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return shearableCompProps.woolAmount;
        }

        return 0m;
    }
}
