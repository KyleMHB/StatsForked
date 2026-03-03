using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Turret;

public sealed class BurstsPerRearmColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
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

                    return new NumberCell.Constant(cellValue);
                }
            }
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
