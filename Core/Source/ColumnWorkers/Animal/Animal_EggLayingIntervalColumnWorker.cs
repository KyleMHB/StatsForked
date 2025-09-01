using RimWorld;

namespace Stats;

public sealed class Animal_EggLayingIntervalColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_EggLayingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            return eggLayerCompProps.eggLayIntervalDays.ToDecimal(1);
        }

        return 0m;
    }
}
