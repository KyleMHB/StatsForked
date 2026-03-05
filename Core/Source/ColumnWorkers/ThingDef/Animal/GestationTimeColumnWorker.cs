using RimWorld;
using Stats.TableCells;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class GestationTimeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                decimal cellValue = AnimalProductionUtility.GestationDaysLitter(thingDef).ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0 d");
            }

        }

        return default;
    }
}
