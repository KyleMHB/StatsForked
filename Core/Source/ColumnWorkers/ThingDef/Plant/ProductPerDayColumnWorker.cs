using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class ProductPerDayColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps is { growDays: > 0f })
            {
                decimal cellValue = (plantProps.harvestYield / plantProps.GetGrowDaysActual()).ToDecimal(2);

                return new NumberTableCell(cellValue, "0.00/d");
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
