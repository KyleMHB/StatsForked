using RimWorld;
using Verse;

namespace Stats;

public sealed class Animal_WoolTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Animal_WoolTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetCompProperties<CompProperties_Shearable>()?.woolDef;
    }
}
