namespace Stats;

public sealed class Weapon_RangeColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_RangeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;

        return thingDef.Verbs.Primary()?.range.ToDecimal(0) ?? 0m;
    }
}
