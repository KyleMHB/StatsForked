using RimWorld;

namespace Stats;

public sealed class Animal_GestationTimeColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Animal_GestationTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return AnimalProductionUtility.GestationDaysLitter(thing.Def).ToDecimal(1);
        }

        return 0m;
    }
}
