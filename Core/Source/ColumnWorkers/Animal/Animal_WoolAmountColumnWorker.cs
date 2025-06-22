using RimWorld;

namespace Stats;

public sealed class Animal_WoolAmountColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Animal_WoolAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override ThingDefCount? GetValue(ThingAlike thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return new(shearableCompProps.woolDef, shearableCompProps.woolAmount);
        }

        return null;
    }
}
