namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class CanBePlantedUnderRoofColumnWorker(ColumnDef columnDef) : BooleanColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override bool GetValue(DefBasedObject @object)
    {
        return @object.Def is Verse.ThingDef { plant.interferesWithRoof: false };
    }
}
