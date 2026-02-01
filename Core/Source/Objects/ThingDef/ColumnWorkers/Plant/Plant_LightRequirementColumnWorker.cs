using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_LightRequirementColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_LightRequirementColumnWorker(ColumnDef columDef) : base(columDef, formatString: "0\\%")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.growMinGlow > 0f)
        {
            return (100f * plantProps.growMinGlow).ToDecimal(0);
        }

        return 0m;
    }
}
