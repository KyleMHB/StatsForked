using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class ProductPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps is { growDays: > 0f })
            {
                decimal cellValue = (plantProps.harvestYield / plantProps.GetGrowDaysActual()).ToDecimal(2);

                return new NumberCell(cellValue, "0.00/d");
            }
        }

        return default;
    }
}
