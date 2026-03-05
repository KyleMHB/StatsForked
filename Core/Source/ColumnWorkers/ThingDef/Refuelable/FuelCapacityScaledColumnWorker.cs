using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;
// Turret rearm cost
public sealed class FuelCapacityScaledColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, FuelCapacityScaledColumnWorker.TableCell>
{
    public override ColumnDef Def => columnDef;

    protected override TableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps != null)
            {
                Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

                if (fuelType != null)
                {
                    decimal fuelCapacity = GetFuelCapacity(refuelableCompProps);

                    return new TableCell(fuelType, fuelCapacity, refuelableCompProps);
                }
            }
        }

        return default;
    }

    protected override TableCell RefreshCell(TableCell cell, out bool wasStale)
    {
        if (cell.RefuelableCompProps != null)
        {
            decimal fuelCapacity = GetFuelCapacity(cell.RefuelableCompProps);
            if (cell.Count != fuelCapacity)
            {
                wasStale = true;
                return new TableCell(cell.ThingDef, fuelCapacity, cell.RefuelableCompProps);
            }
        }

        wasStale = false;
        return cell;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();
    }

    private static decimal GetFuelCapacity(CompProperties_Refuelable refuelableCompProps)
    {
        // TODO: FuelMultiplierCurrentDifficulty might be 0
        return Mathf.CeilToInt(refuelableCompProps.fuelCapacity / refuelableCompProps.FuelMultiplierCurrentDifficulty);
    }

    public readonly struct TableCell : IThingDefCountTableCell
    {
        public Verse.ThingDef? ThingDef => _innerCell.ThingDef;
        public string ThingDefLabel => _innerCell.ThingDefLabel;
        public decimal Count => _innerCell.Count;
        public float Width => _innerCell.Width;

        public readonly CompProperties_Refuelable? RefuelableCompProps;

        private readonly ThingDefCountTableCell _innerCell;

        public TableCell(Verse.ThingDef fuelType, decimal fuelCapacity, CompProperties_Refuelable refuelableCompProps)
        {
            RefuelableCompProps = refuelableCompProps;
            _innerCell = new ThingDefCountTableCell(fuelType, fuelCapacity);
        }

        public void Draw(Rect rect)
        {
            _innerCell.Draw(rect);
        }
    }
}
