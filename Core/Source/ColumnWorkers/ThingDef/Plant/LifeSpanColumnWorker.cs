using RimWorld;
using Stats.Extensions;
using Stats.ColumnWorkers.Cells;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class LifeSpanColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.LifespanDays > 0f)
            {
                decimal cellValue = plantProps.LifespanDays.ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0 d");
            }
        }

        return default;
    }
}
