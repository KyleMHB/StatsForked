namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class IsBlockedByFencesColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        return @object.Def is Verse.ThingDef { race.FenceBlocked: true };
    }
}
