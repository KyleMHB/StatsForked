using CombatExtended;
using RimWorld;

namespace Stats.Compat.CE;

public sealed class Weapon_IsOneHandedColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Weapon_IsOneHandedColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        var statReq = StatRequest.For(thing.Def, thing.StuffDef);

        if (CE_StatDefOf.OneHandedness.Worker.ShouldShowFor(statReq))
        {
            return CE_StatDefOf.OneHandedness.Worker.GetValue(statReq) > 0f;
        }

        return false;
    }
}
