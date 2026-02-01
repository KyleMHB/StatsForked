using RimWorld;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_WoolAmountColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public Animal_WoolAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(VirtualThing thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return new(shearableCompProps.woolDef, shearableCompProps.woolAmount);
        }

        return new();
    }
}
