using RimWorld;
using Verse;

namespace Stats;

public sealed class Animal_WoolAmountColumnWorker : ThingDefCountColumnWorker<AbstractThing>
{
    public Animal_WoolAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(AbstractThing thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return new(shearableCompProps.woolDef, shearableCompProps.woolAmount);
        }

        return new();
    }
}
