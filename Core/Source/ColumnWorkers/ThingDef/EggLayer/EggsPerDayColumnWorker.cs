using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggsPerDayColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
        {
            decimal cellValue = (eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays).ToDecimal(1);

            return new NumberCell.Constant(cellValue, "0.0/d");
        }

        return NumberCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberCell.GetDescriptor(columnDef);
}
