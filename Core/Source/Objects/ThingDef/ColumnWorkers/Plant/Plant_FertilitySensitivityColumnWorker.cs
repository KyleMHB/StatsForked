using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_FertilitySensitivityColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_FertilitySensitivityColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.fertilitySensitivity > 0f)
        {
            return (100f * plantProps.fertilitySensitivity).ToDecimal(0);
        }

        return 0m;
    }
}
