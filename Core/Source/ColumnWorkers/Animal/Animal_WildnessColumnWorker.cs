using Verse;

namespace Stats;

public sealed class Animal_WildnessColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Animal_WildnessColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            var wildness = raceProps.wildness;

            if (wildness > 0f)
            {
                return wildness.ToStringPercent();
            }
        }

        return "";
    }
}
