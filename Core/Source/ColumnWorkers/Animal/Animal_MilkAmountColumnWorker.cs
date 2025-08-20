using RimWorld;
using Verse;

namespace Stats;

public sealed class Animal_MilkAmountColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Animal_MilkAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(ThingAlike thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return new(milkableCompProps.milkDef, milkableCompProps.milkAmount);
        }

        return new();
    }
}
