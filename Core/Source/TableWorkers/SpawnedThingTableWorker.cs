using System.Collections.Generic;
using Verse;

namespace Stats;

public abstract class SpawnedThingTableWorker : TableWorker<ThingAlike>
{
    protected sealed override IEnumerable<ThingAlike> Records
    {
        get
        {
            foreach (var map in Find.Maps)
            {
                foreach (var thing in map.spawnedThings)
                {
                    if (IsValidThing(thing))
                    {
                        yield return new(thing);
                    }
                }
            }
        }
    }
    public SpawnedThingTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected abstract bool IsValidThing(Thing thing);
}
