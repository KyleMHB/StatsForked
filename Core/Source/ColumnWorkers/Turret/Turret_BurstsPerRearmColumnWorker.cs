using RimWorld;

namespace Stats;

public sealed class Turret_BurstsPerRearmColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Turret_BurstsPerRearmColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var turretGunDef = thing.Def.building?.turretGunDef;

        if (turretGunDef != null)
        {
            var refuelableCompProps = thing.Def.GetCompProperties<CompProperties_Refuelable>();

            if (refuelableCompProps is { fuelCapacity: > 0f })
            {
                var turretGunDefPrimaryVerb = turretGunDef.Verbs.Primary();

                if (turretGunDefPrimaryVerb != null)
                {
                    var fuelPerBurst = turretGunDefPrimaryVerb.consumeFuelPerBurst;
                    var fuelPerShot = turretGunDefPrimaryVerb.consumeFuelPerShot;

                    if (fuelPerShot > 0f)
                    {
                        fuelPerBurst = fuelPerShot * turretGunDefPrimaryVerb.burstShotCount;
                    }

                    if (fuelPerBurst > 0f)
                    {
                        return (refuelableCompProps.fuelCapacity / fuelPerBurst).ToDecimal(0);
                    }
                }
            }
        }

        return 0m;
    }
}
