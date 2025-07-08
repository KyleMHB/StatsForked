namespace Stats;

public sealed class Animal_NuzzleIntervalColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_NuzzleIntervalColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 h")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return raceProps.nuzzleMtbHours.ToDecimal(1);
        }

        return 0m;
    }
}
