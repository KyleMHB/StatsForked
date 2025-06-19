using RimWorld;

namespace Stats;

public sealed class Animal_MilkAmountColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_MilkAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return milkableCompProps.milkAmount;
        }

        return 0m;
    }
}
