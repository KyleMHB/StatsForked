namespace Stats;

public sealed class Weapon_RangeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_RangeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        return thing.Def.Verbs.Primary()?.range.ToString("F0") ?? "";
    }
}
