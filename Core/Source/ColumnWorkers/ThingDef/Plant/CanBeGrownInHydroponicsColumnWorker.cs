namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class CanBeGrownInHydroponicsColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            return thingDef.plant?.sowTags.Contains("Hydroponic") == true;
        }

        return default;
    }
}
