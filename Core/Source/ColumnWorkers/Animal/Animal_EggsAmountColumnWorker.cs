using RimWorld;

namespace Stats;

public sealed class Animal_EggsAmountColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Animal_EggsAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override ThingDefCount? GetValue(ThingAlike thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            var eggDef = eggLayerCompProps.GetAnyEggDef();
            var count = eggLayerCompProps.eggCountRange.Average.ToDecimal(0);

            return new(eggDef, count);
        }

        return null;
    }
}
