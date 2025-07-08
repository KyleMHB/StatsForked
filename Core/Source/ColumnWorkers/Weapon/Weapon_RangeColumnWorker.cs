namespace Stats;

public sealed class Weapon_RangeColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Weapon_RangeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;

        return thingDef.Verbs.Primary()?.range.ToDecimal(0) ?? 0m;
    }
}
