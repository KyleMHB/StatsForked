using System.Collections.Generic;
using RimWorld;
using Verse;

namespace Stats;

public abstract class AbstractThingTableWorker : TableWorker<AbstractThing>, TableWorker.IReferenceObjectsProvider<AbstractThing>
{
    private static readonly Dictionary<StuffCategoryDef, HashSet<ThingDef>> StuffsByCategory = [];
    static AbstractThingTableWorker()
    {
        foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.stuffProps == null)
            {
                continue;
            }

            foreach (var stuffCategoryDef in thingDef.stuffProps.categories)
            {
                var exists = StuffsByCategory.TryGetValue(stuffCategoryDef, out var categoryStuffs);

                if (exists)
                {
                    categoryStuffs.Add(thingDef);
                }
                else
                {
                    StuffsByCategory[stuffCategoryDef] = [thingDef];
                }
            }
        }
    }
    public sealed override IEnumerable<AbstractThing> InitialObjects
    {
        get
        {
            // Remember that each stuff can belong to multiple categories
            // and each thing can be crafted from multiple categories of stuff.
            var allowedStuffsFor = new Dictionary<ThingDef, HashSet<ThingDef>>();

            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (IsValidThingDef(thingDef) == false)
                {
                    continue;
                }

                if (thingDef.stuffCategories == null || thingDef.stuffCategories.Count == 0)
                {
                    yield return new AbstractThing(thingDef);
                }
                else
                {
                    var allowedStuffsForEntryExists = allowedStuffsFor.TryGetValue(thingDef, out var allowedStuffs);

                    if (allowedStuffsForEntryExists == false)
                    {
                        allowedStuffs = [];

                        foreach (var stuffCategoryDef in thingDef.stuffCategories)
                        {
                            allowedStuffs.AddRange(StuffsByCategory[stuffCategoryDef]);
                        }

                        allowedStuffsFor[thingDef] = allowedStuffs;
                    }

                    foreach (var stuffDef in allowedStuffsFor[thingDef])
                    {
                        yield return new AbstractThing(thingDef, stuffDef);
                    }
                }
            }
        }
    }
    public IEnumerable<AbstractThing> ReferenceObjects => InitialObjects;
    protected AbstractThingTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected abstract bool IsValidThingDef(ThingDef thingDef);
}
