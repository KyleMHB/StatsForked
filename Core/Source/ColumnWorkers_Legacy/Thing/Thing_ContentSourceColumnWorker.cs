using Stats.Objects.ThingDef;
using Verse;

namespace Stats;

public sealed class Thing_ContentSourceColumnWorker : ContentSourceColumnWorker<VirtualThing>
{
    public Thing_ContentSourceColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ModContentPack? GetModContentPack(VirtualThing thing)
    {
        return thing.Def.modContentPack;
    }
}
