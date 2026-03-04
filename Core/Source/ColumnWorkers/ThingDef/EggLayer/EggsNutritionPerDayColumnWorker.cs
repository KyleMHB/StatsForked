using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.EggLayer;

public sealed class EggsNutritionPerDayColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();

            if (eggLayerCompProps is { eggLayIntervalDays: > 0f })
            {
                Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();
                float eggNutrition = eggDef.GetStatValuePerceived(StatDefOf.Nutrition);
                float eggsPerDay = eggLayerCompProps.eggCountRange.Average / eggLayerCompProps.eggLayIntervalDays;
                decimal cellValue = (eggsPerDay * eggNutrition).ToDecimal(2);

                return new NumberTableCell(cellValue, "0.00/d");
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
