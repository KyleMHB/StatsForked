using CombatExtended;

namespace Stats.Compat.CE;

public sealed class Weapon_IsOneHandedColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Weapon_IsOneHandedColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.GetStatValuePerceived(CE_StatDefOf.OneHandedness, thing.StuffDef) > 0f;
    }
}
