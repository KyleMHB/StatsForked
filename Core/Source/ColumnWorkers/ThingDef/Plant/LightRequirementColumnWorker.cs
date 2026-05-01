using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class LightRequirementColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.growMinGlow > 0f)
            {
                decimal cellValue = (100f * plantProps.growMinGlow).ToDecimal(0);

                return new NumberCell(cellValue, "0\\%");
            }
        }

        return default;
    }
}
