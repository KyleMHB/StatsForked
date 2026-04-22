using Stats.ObjectTable.ColumnWorkers;
using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech;

[StaticConstructorOnStartup]
public sealed class Gene_ComplexityColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D ComplexityIcon;
    static Gene_ComplexityColumnWorker()
    {
        ComplexityIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/Complexity");
    }
    public Gene_ComplexityColumnWorker(ColumnDef columnDef) : base(columnDef, ComplexityIcon)
    {
    }
    protected override decimal GetCellValueSource(GeneDef geneDef)
    {
        return geneDef.biostatCpx;
    }
}
