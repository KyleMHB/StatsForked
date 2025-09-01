using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Animal_ProductsColumnWorker : ThingDefSetColumnWorker<AbstractThing, ThingDef>
{
    public Animal_ProductsColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
        //GetProductCategories = FunctionExtensions.Memoized((ThingAlike thing) =>
        //{
        //    var products = GetCachedValue(thing);

        //    return products.SelectMany(product => product.thingCategories).ToHashSet();
        //});
    }
    protected override HashSet<ThingDef> GetValue(AbstractThing thing)
    {
        var products = new HashSet<ThingDef>();

        var milkableCompProps = thing.Def.GetCompProperties<CompProperties_Milkable>();
        if (milkableCompProps != null)
        {
            products.Add(milkableCompProps.milkDef);
        }

        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();
        if (eggLayerCompProps != null)
        {
            var eggDef = eggLayerCompProps.GetAnyEggDef();

            products.Add(eggDef);
        }

        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();
        if (shearableCompProps != null)
        {
            products.Add(shearableCompProps.woolDef);
        }

        return products;
    }
    //private readonly Func<ThingAlike, HashSet<ThingCategoryDef>> GetProductCategories;
    //public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    //{
    //    yield return new(new Label("Type"), Make.MTMThingDefFilter(GetCachedValue, tableRecords));
    //    // TODO: Implement proper category filter (like the one stockpile uses).
    //    yield return new(new Label("Category"), Make.MTMDefFilter(GetProductCategories, tableRecords));
    //}
}
