using Verse;

namespace Stats;

public sealed class Weapon_RangedDirectHitChanceColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_RangedDirectHitChanceColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return (1f / GenRadial.NumCellsInRadius(verb.ForcedMissRadius)).ToStringPercent();
        }

        return "";
    }
}
