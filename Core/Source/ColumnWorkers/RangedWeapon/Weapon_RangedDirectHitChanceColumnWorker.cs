using Verse;

namespace Stats;

public sealed class Weapon_RangedDirectHitChanceColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_RangedDirectHitChanceColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0\\%")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();

        if (verb != null)
        {
            if (verb.ForcedMissRadius > 0f)
            {
                return (100f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius)).ToDecimal(1);
            }
            else
            {
                return 100m;
            }
        }

        return 0m;
    }
}
