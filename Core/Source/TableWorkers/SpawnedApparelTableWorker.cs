using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Verse;

namespace Stats;

public sealed class SpawnedApparelTableWorker : SpawnedThingTableWorker
{
    public SpawnedApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override bool IsValidThing(Thing thing)
    {
        return thing.def.IsApparel;
    }
}
