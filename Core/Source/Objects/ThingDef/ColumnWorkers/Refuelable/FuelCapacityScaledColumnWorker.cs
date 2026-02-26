using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using UnityEngine;

namespace Stats.Objects.ThingDef.ColumnWorkers.Refuelable;
// Turret rearm cost
public sealed class FuelCapacityScaledColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
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

        return ThingDefCountCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> fuelDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();

        return ThingDefCountCell.GetDescriptor(fuelDefs);
    }
}
