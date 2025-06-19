using RimWorld;

namespace Stats;

public sealed class Building_RecreationTypeColumnWorker : DefColumnWorker<ThingAlike, JoyKindDef?>
{
    public Building_RecreationTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override JoyKindDef? GetValue(ThingAlike thing)
    {
        return thing.Def.building?.joyKind;
    }
}
