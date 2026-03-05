using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggsPerDayColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

            if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
            {
                decimal cellValue = (eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays).ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0/d");
            }
        }

        return default;
    }
}
