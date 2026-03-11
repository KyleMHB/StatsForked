using RimWorld;
using Stats.Extensions;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class LightRequirementColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.growMinGlow > 0f)
            {
                decimal cellValue = (100f * plantProps.growMinGlow).ToDecimal(0);

                return new NumberTableCell(cellValue, "0\\%");
            }
        }

        return default;
    }
}
