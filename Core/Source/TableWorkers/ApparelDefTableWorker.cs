using System.Collections.Generic;
using Verse;

namespace Stats;

// We do not check for "destroyOnDrop" for better compatibility with mods like
// VFE - Pirates.
public sealed class ApparelDefTableWorker : TableWorker<ApparelDef>
{
    public override IEnumerable<ApparelDef> InitialObjects
    {
        get
        {
            foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
            {
                if (thingDef.apparel != null)
                {
                    var stuffDefs = thingDef.GetAllowedStuffs();

                    if (stuffDefs?.Count > 0)
                    {
                        foreach (var stuffDef in stuffDefs)
                        {
                            yield return new ApparelDef(thingDef, thingDef.apparel, stuffDef);
                        }
                    }
                    else
                    {
                        yield return new ApparelDef(thingDef, thingDef.apparel);
                    }
                }
            }
        }
    }
    public ApparelDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
