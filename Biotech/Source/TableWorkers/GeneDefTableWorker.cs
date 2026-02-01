using System.Collections.Generic;
using Stats.ObjectTable;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class GeneDefTableWorker : TableWorker<GeneDef>
{
    public override IEnumerable<GeneDef> InitialObjects => DefDatabase<GeneDef>.AllDefs;
    public GeneDefTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
