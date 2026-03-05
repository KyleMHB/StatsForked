namespace Stats.ColumnWorkers.ThingDef;

public sealed class HasInteractionCellColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            return thingDef.hasInteractionCell;
        }

        return default;
    }
}
