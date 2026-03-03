using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class FertilityRequirementColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps?.fertilityMin > 0f)
        {
            decimal cellValue = (100F * plantProps.fertilityMin).ToDecimal(1);

            return new NumberCell.Constant(cellValue, "0\\%");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
