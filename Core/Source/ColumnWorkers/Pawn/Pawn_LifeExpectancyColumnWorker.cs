namespace Stats;

public sealed class Pawn_LifeExpectancyColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Pawn_LifeExpectancyColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0 y")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        return thing.Def.race?.lifeExpectancy.ToDecimal(0) ?? 0m;
    }
}
