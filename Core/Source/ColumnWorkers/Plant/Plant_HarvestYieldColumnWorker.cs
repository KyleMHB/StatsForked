using UnityEngine;

namespace Stats;

public sealed class Plant_HarvestYieldColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Plant_HarvestYieldColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override ThingDefCount? GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps is { harvestYield: > 0f, harvestedThingDef: not null })
        {
            var yield = Mathf.CeilToInt(plantProps.harvestYield);

            return new(plantProps.harvestedThingDef, yield);
        }

        return null;
    }
}
