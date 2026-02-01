using System.Collections.Generic;
using Stats;
using Stats.Objects.Apparel;
using Stats.ObjectTable.TableWorkers;
using Verse;

namespace Stats.Objects.ThingDef.TableWorkers;

// We do not check for "destroyOnDrop" for better compatibility with mods like
// VFE - Pirates.
public sealed class ApparelTableWorker : TableWorker<ApparelDef>
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
    public ApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
