using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_WoolShearingIntervalColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Animal_WoolShearingIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var shearableCompProps = thing.Def.GetCompProperties<CompProperties_Shearable>();

        if (shearableCompProps != null)
        {
            return shearableCompProps.shearIntervalDays;
        }

        return 0m;
    }
}
