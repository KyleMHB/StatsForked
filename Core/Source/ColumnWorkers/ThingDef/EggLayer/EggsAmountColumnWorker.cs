using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggsAmountColumnWorker(ColumnDef columnDef) : ThingDefCountColumnWorker<DefBasedObject, ThingDefCountTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefCountTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

            if (eggLayerCompProps != null)
            {
                Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
                decimal count = eggLayerCompProps.eggCountRange.Average.ToDecimal(0);

                return new ThingDefCountTableCell(eggDef, count);
            }
        }

        return default;
    }

    protected override IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.GetCompProperties<CompProperties_EggLayer>()?.GetAnyEggDef())
            .Distinct();
    }
}
