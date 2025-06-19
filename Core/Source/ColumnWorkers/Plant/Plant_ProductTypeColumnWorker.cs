using Verse;

namespace Stats;

public sealed class Plant_ProductTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Plant_ProductTypeColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.plant?.harvestedThingDef;
    }
}
