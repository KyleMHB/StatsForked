using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_GrowingTimeActualColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_GrowingTimeActualColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.growDays > 0f)
        {
            return plantProps.GetGrowDaysActual().ToDecimal(1);
        }

        return 0m;
    }
}
