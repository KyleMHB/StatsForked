using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers.ThingDef.Plant;

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

        return ThingDefCountTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> harvestedThingDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.plant?.harvestedThingDef)
            .Distinct();

        return ThingDefCountTableCell.GetDescriptor(harvestedThingDefs);
    }
}
