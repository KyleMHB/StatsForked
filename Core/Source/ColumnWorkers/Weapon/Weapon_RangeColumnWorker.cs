namespace Stats;

public sealed class Weapon_RangeColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_RangeColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;

        return thingDef.Verbs.Primary()?.range.ToString("F0") ?? "";
    }
}
