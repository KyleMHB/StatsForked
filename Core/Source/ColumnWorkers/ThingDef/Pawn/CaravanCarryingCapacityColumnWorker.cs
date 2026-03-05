using RimWorld;
using Stats.TableCells;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Pawn;

public sealed class CaravanCarryingCapacityColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                decimal cellValue = (raceProps.baseBodySize * MassUtility.MassCapacityPerBodySize).ToDecimal(0);

                return new NumberTableCell(cellValue, "0 kg");
            }
        }

        return default;
    }
}
