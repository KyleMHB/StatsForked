namespace Stats;

public sealed class Turret_IsMannedColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Turret_IsMannedColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def is { building.IsTurret: true, hasInteractionCell: true };
    }
}
