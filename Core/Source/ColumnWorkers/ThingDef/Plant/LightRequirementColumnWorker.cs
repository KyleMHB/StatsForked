using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class LightRequirementColumnWorker(ColumnDef columDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        PlantProperties? plantProps = thingDef.plant;

        if (plantProps?.growMinGlow > 0f)
        {
            decimal cellValue = (100f * plantProps.growMinGlow).ToDecimal(0);

            return new NumberCell.Constant(cellValue, "0\\%");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columDef);
}
