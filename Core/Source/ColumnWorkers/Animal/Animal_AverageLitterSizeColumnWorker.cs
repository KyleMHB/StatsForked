using RimWorld;

namespace Stats;

public sealed class Animal_AverageLitterSizeColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_AverageLitterSizeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        if (thing.Def.race != null)
        {
            return AnimalProductionUtility.OffspringRange(thing.Def).Average.ToDecimal(1);
        }

        return 0m;
    }
}
