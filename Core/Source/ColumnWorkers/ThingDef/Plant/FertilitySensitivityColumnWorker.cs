using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class FertilitySensitivityColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps?.fertilitySensitivity > 0f)
        {
            decimal cellValue = (100f * plantProps.fertilitySensitivity).ToDecimal(0);

            return new NumberCell.Constant(cellValue, "0\\%");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
