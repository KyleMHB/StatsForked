using Stats.ColumnWorkers.Def;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Gene_ContentSourceColumnWorker : ContentSourceColumnWorker<GeneDef>
{
    public Gene_ContentSourceColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ModContentPack? GetModContentPack(GeneDef geneDef)
    {
        return geneDef.modContentPack;
    }
}
