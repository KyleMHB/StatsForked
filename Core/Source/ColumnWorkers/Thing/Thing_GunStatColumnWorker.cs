namespace Stats;

public sealed class Thing_GunStatColumnWorker : Thing_StatColumnWorker
{
    public Thing_GunStatColumnWorker(StatColumnDef columnDef) : base(columnDef)
    {
    }
    protected override string? GetStatDrawEntryLabel(ThingAlike thing)
    {
        var turretGunDef = thing.Def.building?.turretGunDef;

        if (turretGunDef != null)
        {
            return base.GetStatDrawEntryLabel(new(turretGunDef));
        }

        return base.GetStatDrawEntryLabel(thing);
    }
    protected override string? GetStatValueExplanation(ThingAlike thing)
    {
        var turretGunDef = thing.Def.building?.turretGunDef;

        if (turretGunDef != null)
        {
            return base.GetStatValueExplanation(new(turretGunDef));
        }

        return base.GetStatValueExplanation(thing);
    }
}
