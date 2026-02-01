using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_FertilityRequirementColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_FertilityRequirementColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.fertilityMin > 0f)
        {
            return (100F * plantProps.fertilityMin).ToDecimal(1);
        }

        return 0m;
    }
}
