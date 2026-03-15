using RimWorld;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class MinGrowingSkillToSowColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps != null)
            {
                return new NumberCell(plantProps.sowMinSkill);
            }
        }

        return default;
    }
}
