using UnityEngine;

namespace Stats;

public sealed class Plant_HarvestYieldColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_HarvestYieldColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null)
        {
            return 0m;
        }

        return Mathf.CeilToInt(plantProps.harvestYield);
    }
}
