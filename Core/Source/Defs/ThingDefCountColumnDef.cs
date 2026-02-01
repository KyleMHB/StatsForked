using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

public class ThingDefCountColumnDef : ColumnDef
{
#pragma warning disable CS8618
    public Func<IEnumerable<ThingDef?>> getThingDefs;
#pragma warning restore CS8618
}
