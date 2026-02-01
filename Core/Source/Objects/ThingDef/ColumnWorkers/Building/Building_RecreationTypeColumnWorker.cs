using RimWorld;
using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Building_RecreationTypeColumnWorker : DefColumnWorker<VirtualThing, JoyKindDef?>
{
    public Building_RecreationTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override JoyKindDef? GetValue(VirtualThing thing)
    {
        return thing.Def.building?.joyKind;
    }
}
