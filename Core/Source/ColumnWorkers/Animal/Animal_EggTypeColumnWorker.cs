using RimWorld;
using Verse;

namespace Stats;

public sealed class Animal_EggTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Animal_EggTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        var eggLayerCompProps = thing.Def.GetCompProperties<CompProperties_EggLayer>();

        if (eggLayerCompProps != null)
        {
            return eggLayerCompProps.eggUnfertilizedDef ?? eggLayerCompProps.eggFertilizedDef;
        }

        return null;
    }
}
