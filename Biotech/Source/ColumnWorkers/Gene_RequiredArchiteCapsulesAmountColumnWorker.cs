using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech;

[StaticConstructorOnStartup]
public sealed class Gene_RequiredArchiteCapsulesAmountColumnWorker : NumberColumnWorker<GeneDef>
{
    private static readonly Texture2D ArchiteCapsuleIcon;
    static Gene_RequiredArchiteCapsulesAmountColumnWorker()
    {
        ArchiteCapsuleIcon = ContentFinder<Texture2D>.Get("UI/Icons/Biostats/ArchiteCapsuleRequired");
    }
    public Gene_RequiredArchiteCapsulesAmountColumnWorker(ColumnDef columnDef) : base(columnDef, ArchiteCapsuleIcon)
    {
    }
    protected override decimal GetValue(GeneDef geneDef)
    {
        return geneDef.biostatArc;
    }
}
