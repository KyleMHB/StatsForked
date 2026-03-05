using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class HarvestYieldColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps is { harvestYield: > 0f, harvestedThingDef: not null })
            {
                decimal yield = Mathf.CeilToInt(plantProps.harvestYield);

                return new ThingDefCountTableCell(plantProps.harvestedThingDef, yield);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.plant?.harvestedThingDef)
            .Distinct();
    }
}
