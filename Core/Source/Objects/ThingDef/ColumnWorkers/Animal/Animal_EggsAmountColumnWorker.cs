using RimWorld;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_EggsAmountColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public Animal_EggsAmountColumnWorker(ColumnDef columndef) : base(columndef)
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
