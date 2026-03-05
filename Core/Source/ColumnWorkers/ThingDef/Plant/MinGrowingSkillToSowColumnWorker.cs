using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class MinGrowingSkillToSowColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps != null)
            {
                return new NumberTableCell(plantProps.sowMinSkill);
            }
        }

        return default;
    }
}
