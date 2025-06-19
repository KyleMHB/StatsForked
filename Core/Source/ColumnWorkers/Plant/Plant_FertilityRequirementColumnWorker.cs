using Verse;

namespace Stats;

public sealed class Plant_FertilityRequirementColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Plant_FertilityRequirementColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null || plantProps.fertilityMin == 0f)
        {
            return "";
        }

        return plantProps.fertilityMin.ToStringPercent();
    }
}
