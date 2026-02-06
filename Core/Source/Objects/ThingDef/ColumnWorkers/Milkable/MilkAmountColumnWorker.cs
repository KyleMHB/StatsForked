using RimWorld;
using Stats.ColumnWorkers_Legacy;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Milkable;

public sealed class MilkAmountColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public MilkAmountColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(VirtualThing thing)
    {
        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();

        if (milkableCompProps != null)
        {
            return new(milkableCompProps.milkDef, milkableCompProps.milkAmount);
        }

        return new();
    }
}
