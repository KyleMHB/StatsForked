using RimWorld;

namespace Stats;

public sealed class Pawn_CaravanCarryingCapacityColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Pawn_CaravanCarryingCapacityColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 kg")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return (raceProps.baseBodySize * MassUtility.MassCapacityPerBodySize).ToDecimal(0);
        }

        return 0m;
    }
}
