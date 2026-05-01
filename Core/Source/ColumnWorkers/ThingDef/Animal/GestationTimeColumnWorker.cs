using RimWorld;
using Verse;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class GestationTimeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                decimal cellValue = AnimalProductionUtility.GestationDaysLitter(thingDef).ToDecimal(1);

                return new NumberCell(cellValue, "0.0 d");
            }

        }

        return default;
    }
}
