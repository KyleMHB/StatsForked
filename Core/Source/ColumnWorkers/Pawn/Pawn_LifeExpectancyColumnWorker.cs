namespace Stats;

public sealed class Pawn_LifeExpectancyColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Pawn_LifeExpectancyColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 y")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        return thing.Def.race?.lifeExpectancy.ToDecimal(0) ?? 0m;
    }
}
