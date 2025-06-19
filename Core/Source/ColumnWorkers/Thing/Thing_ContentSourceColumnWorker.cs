using Verse;

namespace Stats;

public sealed class Thing_ContentSourceColumnWorker : ContentSourceColumnWorker<ThingAlike>
{
    public Thing_ContentSourceColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ModContentPack? GetModContentPack(ThingAlike thing)
    {
        return thing.Def.modContentPack;
    }
}
