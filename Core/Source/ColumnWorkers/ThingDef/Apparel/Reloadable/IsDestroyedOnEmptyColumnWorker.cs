using RimWorld;

namespace Stats.ColumnWorkers.ThingDef.Apparel.Reloadable;

public sealed class IsDestroyedOnEmptyColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            CompProperties_ApparelReloadable? reloadableCompProperties = thingDef.GetCompProperties<CompProperties_ApparelReloadable>();

            return reloadableCompProperties?.destroyOnEmpty == true;
        }

        return default;
    }
}
