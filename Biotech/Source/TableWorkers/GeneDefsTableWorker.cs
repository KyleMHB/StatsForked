using System.Collections.Generic;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class GeneDefsTableWorker : TableWorker<GeneDef>
{
    protected override IEnumerable<GeneDef> InitialObjects => DefDatabase<GeneDef>.AllDefs;
    public GeneDefsTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
