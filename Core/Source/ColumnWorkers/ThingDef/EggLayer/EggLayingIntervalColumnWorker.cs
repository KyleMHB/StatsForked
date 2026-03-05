using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggLayingIntervalColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

            if (eggLayerCompProps != null)
            {
                decimal cellValue = eggLayerCompProps.eggLayIntervalDays.ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0 d");
            }
        }

        return default;
    }
}
