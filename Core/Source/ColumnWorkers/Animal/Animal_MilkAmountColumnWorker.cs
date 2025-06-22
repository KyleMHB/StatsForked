using RimWorld;

namespace Stats;

public sealed class Animal_MilkAmountColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Animal_MilkAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override ThingDefCount? GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return new(milkableCompProps.milkDef, milkableCompProps.milkAmount);
        }

        return null;
    }
}
