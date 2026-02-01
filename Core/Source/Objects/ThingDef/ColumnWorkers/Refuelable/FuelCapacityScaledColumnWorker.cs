using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using UnityEngine;

namespace Stats.Objects.ThingDef.ColumnWorkers.Refuelable;
// Turret rearm cost
public sealed class FuelCapacityScaledColumnWorker(ThingDefCountColumnDef columnDef) :
    IColumnWorker<Verse.ThingDef>,
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell GetCell(Verse.Thing thing) => GetCell(thing.def);
    public Cell GetCell(VirtualThing thing) => GetCell(thing.Def);
    public Cell GetCell(Verse.ThingDef thingDef)
    {
        CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps != null)
        {
            Verse.ThingDef fuelType = refuelableCompProps.fuelFilter.AnyAllowedDef;

            if (fuelType != null)
            {
                ThingDefCount cellValueSource()
                {
                    // TODO: FuelMultiplierCurrentDifficulty might be 0
                    decimal fuelCapacity = Mathf.CeilToInt(refuelableCompProps.fuelCapacity / refuelableCompProps.FuelMultiplierCurrentDifficulty);

                    return new(fuelType, fuelCapacity);
                }

                return new ThingDefCountCell(cellValueSource);
            }
        }

        return ThingDefCountCell.Empty;
    }
    public CellDescriptor GetCellDescriptor() => ThingDefCountCell.GetDescriptor(columnDef);
}
