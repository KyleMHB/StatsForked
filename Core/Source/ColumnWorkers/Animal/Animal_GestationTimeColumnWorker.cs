using RimWorld;

namespace Stats;

public sealed class Animal_GestationTimeColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_GestationTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var gestationTime = AnimalProductionUtility.GestationDaysLitter(thing.Def);

            if (gestationTime > 0f)
            {
                return gestationTime.ToDecimal(1);
            }
        }

        return 0m;
    }
}
