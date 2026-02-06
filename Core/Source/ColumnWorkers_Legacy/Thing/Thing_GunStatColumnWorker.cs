using Stats.Objects.ThingDef;
using Stats.Widgets;

namespace Stats;

public sealed class Thing_GunStatColumnWorker : StatColumnWorker
{
    public Thing_GunStatColumnWorker(StatColumnDef columnDef) : base(columnDef)
    {
    }
    public override ObjectTableWidget.Cell GetCell(VirtualThing thing)
    {
        var turretGunDef = thing.Def.building?.turretGunDef;

        if (turretGunDef != null)
        {
            thing = new(turretGunDef);
        }

        return base.GetCell(thing);
    }
}
