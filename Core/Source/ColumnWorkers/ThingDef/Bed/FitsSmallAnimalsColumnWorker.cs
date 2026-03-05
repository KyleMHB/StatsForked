namespace Stats.ColumnWorkers.ThingDef.Bed;

public sealed class FitsSmallAnimalsColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        return @object.Def is Verse.ThingDef { building.bed_humanlike: false };
    }
}
