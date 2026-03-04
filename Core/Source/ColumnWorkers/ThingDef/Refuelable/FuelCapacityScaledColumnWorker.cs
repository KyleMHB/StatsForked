using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;
// Turret rearm cost
public sealed class FuelCapacityScaledColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
        }
        CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps != null)
        {
            Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelType != null)
            {
                ThingDefCount cellValueSource()
                {
                    // TODO: FuelMultiplierCurrentDifficulty might be 0
                    decimal fuelCapacity = Mathf.CeilToInt(refuelableCompProps.fuelCapacity / refuelableCompProps.FuelMultiplierCurrentDifficulty);

                    return new ThingDefCount(fuelType, fuelCapacity);
                }

                return new ThingDefCountCell(cellValueSource);
            }
        }

        return ThingDefCountTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> fuelDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();

        return ThingDefCountTableCell.GetDescriptor(fuelDefs);
    }
}
