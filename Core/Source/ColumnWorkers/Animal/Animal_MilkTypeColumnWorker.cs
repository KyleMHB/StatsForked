using RimWorld;
using Verse;

namespace Stats;

public sealed class Animal_MilkTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Animal_MilkTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetCompProperties<CompProperties_Milkable>()?.milkDef;
    }
}
