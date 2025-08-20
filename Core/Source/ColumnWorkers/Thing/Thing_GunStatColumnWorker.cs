namespace Stats;

public sealed class Thing_GunStatColumnWorker : Thing_StatColumnWorker
{
    public Thing_GunStatColumnWorker(StatColumnDef columnDef) : base(columnDef)
    {
    }
    protected override Cell GetCell(ThingAlike thing)
    {
        var turretGunDef = thing.Def.building?.turretGunDef;

        if (turretGunDef != null)
        {
            thing = new(turretGunDef);
        }

        return base.GetCell(thing);
    }
}
