using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class NuzzleIntervalColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            RaceProperties? raceProps = thingDef.race;

            if (raceProps != null)
            {
                decimal cellValue = raceProps.nuzzleMtbHours.ToDecimal(1);

                return new NumberCell(cellValue, "0.0 h");
            }
        }

        return default;
    }
}
