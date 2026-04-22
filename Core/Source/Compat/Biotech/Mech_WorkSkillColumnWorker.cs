using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_WorkSkillColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is ThingDef thingDef)
        {
            int workSkill = thingDef.race?.mechFixedSkillLevel ?? 0;
            if (workSkill != 0)
            {
                return new NumberCell(workSkill);
            }
        }

        return default;
    }
}
