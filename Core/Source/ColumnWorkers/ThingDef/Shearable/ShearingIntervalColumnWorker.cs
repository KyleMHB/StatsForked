using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Shearable;

public sealed class ShearingIntervalColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();

            if (shearableCompProps != null)
            {
                return new NumberTableCell(shearableCompProps.shearIntervalDays, "0 d");
            }
        }

        return default;
    }

    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
