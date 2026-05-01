using CombatExtended;
using Stats.ColumnWorkers;
using Stats.Utils.Extensions;

namespace Stats.Compat.CE;

public sealed class Weapon_IsOneHandedColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        return @object.Def is Verse.ThingDef thingDef
            && thingDef.GetStatValuePerceived(CE_StatDefOf.OneHandedness, @object.StuffDef, @object.Quality) > 0f;
    }
}
