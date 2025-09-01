namespace Stats;

public sealed class Apparel_CountsAsClothingForNudityColumnWorker : BooleanColumnWorker<AbstractThing>
{
    public Apparel_CountsAsClothingForNudityColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override bool GetValue(AbstractThing thing)
    {
        return thing.Def.apparel?.countsAsClothingForNudity ?? false;
    }
}
