using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_RechargerNeededColumnWorker : ThingDefColumnWorker<AbstractThing, ThingDef?>
{
    public Mech_RechargerNeededColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(AbstractThing thing)
    {
        return MechanitorUtility.RechargerForMech(thing.Def);
    }
}
