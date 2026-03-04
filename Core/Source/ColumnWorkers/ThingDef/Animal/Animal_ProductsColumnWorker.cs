using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class Animal_ProductsColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        HashSet<Verse.ThingDef> products = GetProducts(thingDef);

        if (products.Count > 0)
        {
            return new ThingDefSetTableCell(products);
        }

        return ThingDefSetTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef> products = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(GetProducts)
            .Distinct();

        return ThingDefSetTableCell.GetDescriptor(columnDef, products);
    }
    private static HashSet<Verse.ThingDef> GetProducts(Verse.ThingDef thingDef)
    {
        HashSet<Verse.ThingDef> products = new(3);

        var milkableCompProps = thingDef.GetCompProperties<CompProperties_Milkable>();
        if (milkableCompProps != null)
        {
            products.Add(milkableCompProps.milkDef);
        }

        var eggLayerCompProps = thingDef.GetCompProperties<CompProperties_EggLayer>();
        if (eggLayerCompProps != null)
        {
            var eggDef = eggLayerCompProps.GetAnyEggDef();

            products.Add(eggDef);
        }

        var shearableCompProps = thingDef.GetCompProperties<CompProperties_Shearable>();
        if (shearableCompProps != null)
        {
            products.Add(shearableCompProps.woolDef);
        }

        return products;
    }
}
