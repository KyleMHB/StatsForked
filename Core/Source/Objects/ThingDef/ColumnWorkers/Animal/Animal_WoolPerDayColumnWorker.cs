using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_WoolPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Animal_WoolPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps is { shearIntervalDays: > 0 })
        {
            return ((float)shearableCompProps.woolAmount / shearableCompProps.shearIntervalDays).ToDecimal(1);
        }

        return 0m;
    }
}
