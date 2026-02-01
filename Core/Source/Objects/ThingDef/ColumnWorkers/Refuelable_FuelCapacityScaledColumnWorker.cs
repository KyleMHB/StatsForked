using System.Collections.Generic;
using RimWorld;
using Stats.ObjectTable.Cells;
using Stats.ObjectTable.ColumnWorkers;
using UnityEngine;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers;
// Turret rearm cost
public sealed class Refuelable_FuelCapacityScaledColumnWorker : IColumnWorker<TurretDef>
{
    public CellStyleType CellStyle => throw new System.NotImplementedException();
    private readonly ColumnDef ColumnDef;
    public Refuelable_FuelCapacityScaledColumnWorker(ColumnDef columnDef)
    {
        ColumnDef = columnDef;
    }
    public Cell GetCell(TurretDef turretDef)
    {
        CompProperties_Refuelable refuelableCompProps = turretDef.Def.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps is { fuelCapacity: > 0f, FuelMultiplierCurrentDifficulty: > 0f })
        {
            ThingDef fuelThingDef = refuelableCompProps.fuelFilter.AnyAllowedDef;

            if (fuelThingDef != null)
            {
                decimal fuelCapacity = Mathf.CeilToInt(refuelableCompProps.fuelCapacity / refuelableCompProps.FuelMultiplierCurrentDifficulty);

                return new ThingDefCountCell(fuelThingDef, fuelCapacity);
            }
        }

        return new ThingDefCountCell();
    }
    public IEnumerable<ColumnPart> GetCellDescriptor()
    {
        throw new System.NotImplementedException();
    }
}
