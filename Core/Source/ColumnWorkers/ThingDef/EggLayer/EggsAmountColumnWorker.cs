using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggsAmountColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
            decimal count = eggLayerCompProps.eggCountRange.Average.ToDecimal(0);
            ThingDefCount cellValue = new(eggDef, count);

            return new ThingDefCountCell(cellValue);
        }

        return ThingDefCountTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef?> eggDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_EggLayer>()?.GetAnyEggDef())
            .Distinct();

        return ThingDefCountTableCell.GetDescriptor(eggDefs);
    }
}
