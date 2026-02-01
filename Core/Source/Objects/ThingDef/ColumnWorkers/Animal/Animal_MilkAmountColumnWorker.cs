using RimWorld;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_MilkAmountColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public Animal_MilkAmountColumnWorker(ColumnDef columndef) : base(columndef)
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
