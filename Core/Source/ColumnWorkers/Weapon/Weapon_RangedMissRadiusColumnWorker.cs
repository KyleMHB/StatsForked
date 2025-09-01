namespace Stats;

public sealed class Weapon_RangedMissRadiusColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_RangedMissRadiusColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();

        if (verb != null)
        {
            return verb.ForcedMissRadius.ToDecimal(1);
        }

        return 0m;
    }
}
