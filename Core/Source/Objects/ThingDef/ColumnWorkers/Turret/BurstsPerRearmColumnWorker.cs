using RimWorld;
using Stats.ObjectTable.Cells;
using Stats.ObjectTable.ColumnWorkers;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Turret;

public sealed class BurstsPerRearmColumnWorker(ColumnDef columnDef) :
    NumberColumnWorker<Verse.ThingDef>(columnDef),
    IColumnWorker<VirtualThing>,
    IColumnWorker<Verse.Thing>
{
    public Cell GetCell(VirtualThing thing) => GetCell(thing.Def);
    public Cell GetCell(Verse.Thing thing) => GetCell(thing.def);
    protected override CellValueSource<decimal> GetCellValueSource(Verse.ThingDef thingDef)
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

                    return () => cellValue;
                }
            }
        }

        return () => 0m;
    }
}
