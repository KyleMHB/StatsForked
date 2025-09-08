using Verse;

namespace Stats;

public sealed class Weapon_RangedAimingTimeColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Weapon_RangedAimingTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00 " + "LetterSecond".Translate())
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();

        if (verb?.warmupTime > 0f)
        {
            return verb.warmupTime.ToDecimal(2);
        }

        return 0m;
    }
}
