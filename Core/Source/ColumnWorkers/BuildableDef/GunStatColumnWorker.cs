namespace Stats.ColumnWorkers.BuildableDef;

public sealed class GunStatColumnWorker(StatColumnDef columnDef) : StatColumnWorker(columnDef)
{
    protected override StatCell MakeCell(DefBasedObject @object)
    {
        // TODO: What about Thing?
        if (@object.Def is Verse.ThingDef { building.turretGunDef: Verse.ThingDef turretGunDef })
        {
            return base.MakeCell(new DefBasedObject(turretGunDef, quality: @object.Quality));
        }

        return base.MakeCell(@object);
    }
}
