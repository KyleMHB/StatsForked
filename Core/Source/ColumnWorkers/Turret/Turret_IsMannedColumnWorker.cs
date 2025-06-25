namespace Stats;

public sealed class Turret_IsMannedColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Turret_IsMannedColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def is { building.IsTurret: true, hasInteractionCell: true };
    }
}
