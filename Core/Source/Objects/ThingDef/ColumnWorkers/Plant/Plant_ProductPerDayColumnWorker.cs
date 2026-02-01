using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_ProductPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_ProductPerDayColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.00/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps is { growDays: > 0f })
        {
            return (plantProps.harvestYield / plantProps.GetGrowDaysActual()).ToDecimal(2);
        }

        return 0m;
    }
}
