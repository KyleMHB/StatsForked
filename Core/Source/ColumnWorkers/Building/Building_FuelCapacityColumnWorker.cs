using Verse;

namespace Stats;

public sealed class Building_FuelCapacityColumnWorker : ThingDefCountColumnWorker<AbstractThing>
{
    public Building_FuelCapacityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(AbstractThing thing)
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
