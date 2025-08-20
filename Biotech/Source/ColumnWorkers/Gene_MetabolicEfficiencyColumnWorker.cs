using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech;

[StaticConstructorOnStartup]
public sealed class Gene_MetabolicEfficiencyColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D MetabolismIcon;
    static Gene_MetabolicEfficiencyColumnWorker()
    {
        MetabolismIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/Metabolism");
    }
    public Gene_MetabolicEfficiencyColumnWorker(ColumnDef columnDef) : base(columnDef, MetabolismIcon)
    {
    }
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatMet;
    }
}
