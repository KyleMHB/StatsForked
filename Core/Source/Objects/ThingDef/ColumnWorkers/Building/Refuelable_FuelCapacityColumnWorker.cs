using Stats;
using Stats.Objects.ThingDef;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Building;

public sealed class Refuelable_FuelCapacityColumnWorker : ThingDefCountColumnWorker<VirtualThing>
{
    public Refuelable_FuelCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(VirtualThing thing)
    {
        var refuelableCompProps = thing.Def.GetRefuelableCompProperties();

        if (refuelableCompProps != null)
        {
            var fuelDef = refuelableCompProps.fuelFilter?.AnyAllowedDef;

            if (fuelDef != null)
            {
                var fuelCapacity = refuelableCompProps.fuelCapacity.ToDecimal(0);

                return new(fuelDef, fuelCapacity);
            }
        }

        return new();
    }
}
