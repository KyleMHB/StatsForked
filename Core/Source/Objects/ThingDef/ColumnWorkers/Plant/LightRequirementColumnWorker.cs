using RimWorld;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class LightRequirementColumnWorker(ColumnDef columDef) : ThingDefColumnWorker
{
    public override Cell GetCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps?.growMinGlow > 0f)
        {
            decimal cellValue = (100f * plantProps.growMinGlow).ToDecimal(0);

            return new NumberCell(cellValue, "0\\%");
        }

        return NumberCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columDef);
}
