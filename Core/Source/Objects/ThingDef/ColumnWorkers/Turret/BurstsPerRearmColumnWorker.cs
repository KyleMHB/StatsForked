using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Turret;

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

                    return new NumberCell(cellValue);
                }
            }
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
