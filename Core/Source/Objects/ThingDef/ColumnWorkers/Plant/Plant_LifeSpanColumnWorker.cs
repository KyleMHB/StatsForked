using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_LifeSpanColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_LifeSpanColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.LifespanDays > 0f)
        {
            return plantProps.LifespanDays.ToDecimal(1);
        }

        return 0m;
    }
}
