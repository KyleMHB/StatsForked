using Verse;

namespace Stats.Compat.Biotech;

public sealed class Gene_ComplexityColumnWorker(ColumnDef columnDef) : GeneNumberColumnWorker(columnDef)
{
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatCpx;
    }
}
