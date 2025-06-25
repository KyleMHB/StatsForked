namespace Stats;

public sealed class Weapon_RangedMissRadiusColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Weapon_RangedMissRadiusColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var verb = thingDef.Verbs.Primary();

        if (verb?.ForcedMissRadius > 0f)
        {
            return verb.ForcedMissRadius.ToString("0.#");
        }

        return "";
    }
}
