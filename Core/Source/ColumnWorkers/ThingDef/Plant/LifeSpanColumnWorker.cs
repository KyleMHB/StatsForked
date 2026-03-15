using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class LifeSpanColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.LifespanDays > 0f)
            {
                decimal cellValue = plantProps.LifespanDays.ToDecimal(1);

                return new NumberCell(cellValue, "0.0 d");
            }
        }

        return default;
    }
}
