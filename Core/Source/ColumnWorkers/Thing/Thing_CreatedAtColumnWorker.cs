using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class Thing_CreatedAtColumnWorker : ThingDefSetColumnWorker<ThingAlike, ThingDef>
{
    public Thing_CreatedAtColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override HashSet<ThingDef> GetValue(ThingAlike thing)
    {
        var thingCanBeCrafted = ThingCraftingBenches.TryGetValue(thing.Def, out var craftingBenchesDefs);

        if (thingCanBeCrafted)
        {
            return craftingBenchesDefs;
        }

        return [];
    }
    private static readonly Dictionary<ThingDef, HashSet<ThingDef>> ThingCraftingBenches = [];
    static Thing_CreatedAtColumnWorker()
    {
        foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            // See Verse.ThingDef.SpecialDisplayStats()
            if (recipeDef is { products.Count: 1, IsSurgery: false })
            {
                var producedThingDef = recipeDef.products[0]?.thingDef;

                if (producedThingDef != null && recipeDef.recipeUsers?.Count > 0)
                {
                    var craftingBenchesEntryExists = ThingCraftingBenches.TryGetValue(producedThingDef, out var craftingBenches);

                    if (craftingBenchesEntryExists)
                    {
                        craftingBenches.AddRange(recipeDef.recipeUsers);
                    }
                    else
                    {
                        ThingCraftingBenches[producedThingDef] = [.. recipeDef.recipeUsers];
                    }
                }
            }
        }

        foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.recipes?.Count > 0)
            {
                foreach (var recipeDef in thingDef.recipes)
                {
                    if (recipeDef is { products.Count: 1, IsSurgery: false })
                    {
                        var producedThingDef = recipeDef.products[0]?.thingDef;

                        if (producedThingDef != null)
                        {
                            var craftingBenchesEntryExists = ThingCraftingBenches.TryGetValue(producedThingDef, out var craftingBenches);

                            if (craftingBenchesEntryExists)
                            {
                                craftingBenches.Add(thingDef);
                            }
                            else
                            {
                                ThingCraftingBenches[producedThingDef] = [thingDef];
                            }
                        }
                    }
                }
            }
        }
    }
}
