using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;

public sealed class FuelCapacityColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
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
                decimal fuelCapacity = refuelableCompProps.fuelCapacity.ToDecimal(0);
                ThingDefCount cellValue = new(fuelType, fuelCapacity);

                return new ThingDefCountCell(cellValue);
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
