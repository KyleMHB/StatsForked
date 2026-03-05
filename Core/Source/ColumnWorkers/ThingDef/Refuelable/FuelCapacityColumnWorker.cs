using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Refuelable;

public sealed class FuelCapacityColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, ThingDefCountTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps != null)
            {
                Verse.ThingDef? fuelType = refuelableCompProps.fuelFilter?.AnyAllowedDef;

                if (fuelType != null)
                {
                    decimal fuelCapacity = refuelableCompProps.fuelCapacity.ToDecimal(0);

                    return new ThingDefCountTableCell(fuelType, fuelCapacity);
                }
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_Refuelable>()?.fuelFilter?.AnyAllowedDef)
            .Distinct();
    }
}
