using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using UnityEngine;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class HarvestYieldColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps is { harvestYield: > 0f, harvestedThingDef: not null })
        {
            decimal yield = Mathf.CeilToInt(plantProps.harvestYield);
            ThingDefCount cellValue = new(plantProps.harvestedThingDef, yield);

            return new ThingDefCountCell(cellValue);
        }

        return ThingDefCountCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> harvestedThingDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.plant?.harvestedThingDef)
            .Distinct();

        return ThingDefCountCell.GetDescriptor(harvestedThingDefs);
    }
}
