using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class FertilityRequirementColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.fertilityMin > 0f)
            {
                decimal cellValue = (100F * plantProps.fertilityMin).ToDecimal(1);

                return new NumberTableCell(cellValue, "0\\%");
            }
        }

        return default;
    }
}
