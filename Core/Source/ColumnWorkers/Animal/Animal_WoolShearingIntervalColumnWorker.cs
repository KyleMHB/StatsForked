using RimWorld;

namespace Stats;

public sealed class Animal_WoolShearingIntervalColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_WoolShearingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return shearableCompProps.shearIntervalDays;
        }

        return 0m;
    }
}
