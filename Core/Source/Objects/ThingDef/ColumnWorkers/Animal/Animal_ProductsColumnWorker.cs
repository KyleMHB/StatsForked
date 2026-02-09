using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_ProductsColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        HashSet<Verse.ThingDef> products = GetProducts(thingDef);

        if (products.Count > 0)
        {
            return new ThingDefSetCell(products);
        }

        return ThingDefSetCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<Verse.ThingDef> products = ((IRefRecordsProvider)tableWorker).Records
            .SelectMany(GetProducts)
            .Distinct();

        return ThingDefSetCell.GetDescriptor(columnDef, products);
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
