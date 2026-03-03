using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class ProductPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps is { growDays: > 0f })
        {
            decimal cellValue = (plantProps.harvestYield / plantProps.GetGrowDaysActual()).ToDecimal(2);

            return new NumberCell.Constant(cellValue, "0.00/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
