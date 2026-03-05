using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class AverageLitterSizeColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef and { race: not null })
        {
            decimal cellValue = AnimalProductionUtility.OffspringRange(thingDef).Average.ToDecimal(1);

            return new NumberTableCell(cellValue, "0.0");
        }

        return default;
    }
}
