using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class ProductsColumnWorker(ColumnDef columnDef) : ThingDefSetColumnWorker<DefBasedObject, ThingDefSetTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override ThingDefSetTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            HashSet<Verse.ThingDef> products = GetProducts(thingDef);

            if (products.Count > 0)
            {
                return new ThingDefSetTableCell(products);
            }
        }

        return default;
    }

    private static HashSet<Verse.ThingDef> GetProducts(Verse.ThingDef thingDef)
    {
        HashSet<Verse.ThingDef> products = new(3);

        CompProperties_Milkable? milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();
        if (milkableCompProps != null)
        {
            products.Add(milkableCompProps.milkDef);
        }

        CompProperties_EggLayer? eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();
        if (eggLayerCompProps != null)
        {
            Verse.ThingDef eggDef = eggLayerCompProps.GetAnyEggDef();

            products.Add(eggDef);
        }

        CompProperties_Shearable? shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();
        if (shearableCompProps != null)
        {
            products.Add(shearableCompProps.woolDef);
        }

        return products;
    }

    protected override IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .SelectMany(GetProducts)
            .Distinct();
    }
}
