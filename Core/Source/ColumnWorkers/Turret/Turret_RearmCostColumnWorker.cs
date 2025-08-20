using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Turret_RearmCostColumnWorker : ThingDefCountColumnWorker<ThingAlike>
{
    public Turret_RearmCostColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override (ThingDef? Def, decimal Count) GetValue(ThingAlike thing)
    {
        var refuelableCompProps = thing.Def.GetCompProperties<CompProperties_Refuelable>();

        if (refuelableCompProps is { fuelCapacity: > 0f, FuelMultiplierCurrentDifficulty: > 0f })
        {
            var fuelThingDef = refuelableCompProps.fuelFilter.AnyAllowedDef;

            if (fuelThingDef != null)
            {
                var rearmCost = refuelableCompProps.fuelCapacity / refuelableCompProps.FuelMultiplierCurrentDifficulty;

                return new(fuelThingDef, Mathf.CeilToInt(rearmCost));
            }
        }

        return (null, 0m);
    }
}
