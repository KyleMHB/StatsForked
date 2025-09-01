using System.Collections.Generic;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class GeneTableWorker : TableWorker<GeneDef>
{
    protected override IEnumerable<GeneDef> InitialRecords => DefDatabase<GeneDef>.AllDefs;
    public GeneTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
