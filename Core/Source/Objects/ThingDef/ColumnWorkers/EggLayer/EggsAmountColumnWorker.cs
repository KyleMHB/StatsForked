using RimWorld;
using Stats.ColumnWorkers_Legacy;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.EggLayer;

public sealed class EggsAmountColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public EggsAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(VirtualThing thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            var eggDef = eggLayerCompProps.GetAnyEggDef();
            var count = eggLayerCompProps.eggCountRange.Average.ToDecimal(0);

            return new(eggDef, count);
        }

        return new();
    }
}
