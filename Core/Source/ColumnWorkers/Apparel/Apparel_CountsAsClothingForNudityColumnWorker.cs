namespace Stats;

public sealed class Apparel_CountsAsClothingForNudityColumnWorker : BooleanColumnWorker<ThingAlike>
{
    public Apparel_CountsAsClothingForNudityColumnWorker(ColumnDef columnDef) : base(columnDef, false)
    {
    }
    protected override bool GetValue(ThingAlike thing)
    {
        return thing.Def.apparel?.countsAsClothingForNudity ?? false;
    }
}
