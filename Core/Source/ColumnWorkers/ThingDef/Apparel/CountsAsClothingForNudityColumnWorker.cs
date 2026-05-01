namespace Stats.ColumnWorkers.ThingDef.Apparel;

public sealed class CountsAsClothingForNudityColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        return @object.Def is Verse.ThingDef { apparel.countsAsClothingForNudity: true };
    }
}
