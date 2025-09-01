using RimWorld;

namespace Stats;

public sealed class Animal_MilkingIntervalColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_MilkingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return milkableCompProps.milkIntervalDays;
        }

        return 0m;
    }
}
