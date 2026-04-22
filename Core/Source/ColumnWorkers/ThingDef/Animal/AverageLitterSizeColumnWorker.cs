using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Utils.Extensions;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class AverageLitterSizeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef and { race: not null })
        {
            decimal cellValue = AnimalProductionUtility.OffspringRange(thingDef).Average.ToDecimal(1);

            return new NumberCell(cellValue, "0.0");
        }

        return default;
    }
}
