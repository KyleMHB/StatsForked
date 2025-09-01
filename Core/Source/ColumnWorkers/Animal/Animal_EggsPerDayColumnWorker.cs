using RimWorld;

namespace Stats;

public sealed class Animal_EggsPerDayColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_EggsPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            return (eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
