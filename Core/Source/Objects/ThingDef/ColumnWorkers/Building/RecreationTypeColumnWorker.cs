using RimWorld;
using Stats.Objects.ThingDef;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class RecreationTypeColumnWorker : DefColumnWorker<VirtualThing, JoyKindDef?>
{
    public RecreationTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override JoyKindDef? GetValue(VirtualThing thing)
    {
        return thing.Def.building?.joyKind;
    }
}
