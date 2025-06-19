using Verse;

namespace Stats;

public sealed class Building_FuelTypeColumnWorker : ThingDefColumnWorker<ThingAlike, ThingDef?>
{
    public Building_FuelTypeColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override ThingDef? GetValue(ThingAlike thing)
    {
        return thing.Def.GetRefuelableCompProperties()?.fuelFilter?.AnyAllowedDef;
    }
}
