using Verse;

namespace Stats;

public sealed class Thing_ContentSourceColumnWorker : ContentSourceColumnWorker<AbstractThing>
{
    public Thing_ContentSourceColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ModContentPack? GetModContentPack(AbstractThing thing)
    {
        return thing.Def.modContentPack;
    }
}
