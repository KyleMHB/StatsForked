using RimWorld;

namespace Stats;

public sealed class Building_RecreationTypeColumnWorker : DefColumnWorker<AbstractThing, JoyKindDef?>
{
    public Building_RecreationTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override JoyKindDef? GetValue(AbstractThing thing)
    {
        return thing.Def.building?.joyKind;
    }
}
