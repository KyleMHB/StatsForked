using RimWorld;
using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Turret;

public sealed class BurstsPerRearmColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Refuelable? refuelableCompProps = thingDef.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps is { fuelCapacity: > 0f })
            {
                VerbProperties? turretGunDefPrimaryVerbProps = thingDef.building?.turretGunDef?.Verbs.Primary();

                if (turretGunDefPrimaryVerbProps != null)
                {
                    float fuelPerBurst = turretGunDefPrimaryVerbProps.consumeFuelPerBurst;
                    float fuelPerShot = turretGunDefPrimaryVerbProps.consumeFuelPerShot;

                    if (fuelPerShot > 0f)
                    {
                        fuelPerBurst = fuelPerShot * turretGunDefPrimaryVerbProps.burstShotCount;
                    }

                    if (fuelPerBurst > 0f)
                    {
                        decimal cellValue = (refuelableCompProps.fuelCapacity / fuelPerBurst).ToDecimal(0);

                        return new NumberCell(cellValue);
                    }
                }
            }
        }

        return default;
    }
}
