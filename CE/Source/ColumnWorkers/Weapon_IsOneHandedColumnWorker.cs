using CombatExtended;

namespace Stats.Compat.CE;

public sealed class Weapon_IsOneHandedColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Weapon_IsOneHandedColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.GetStatValuePerceived(CE_StatDefOf.OneHandedness, thing.StuffDef) > 0f;
    }
}
