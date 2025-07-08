namespace Stats;

public sealed class Animal_WildnessColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Animal_WildnessColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps?.wildness > 0f)
        {
            return (100f * raceProps.wildness).ToDecimal(0);
        }

        return 0m;
    }
}
