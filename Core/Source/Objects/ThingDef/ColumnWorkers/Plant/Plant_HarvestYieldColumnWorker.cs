using Stats.Objects.ThingDef;
using UnityEngine;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_HarvestYieldColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public Plant_HarvestYieldColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps is { harvestYield: > 0f, harvestedThingDef: not null })
        {
            var yield = Mathf.CeilToInt(plantProps.harvestYield);

            return new(plantProps.harvestedThingDef, yield);
        }

        return new();
    }
}
