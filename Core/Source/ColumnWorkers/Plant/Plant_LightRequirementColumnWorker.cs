using Verse;

namespace Stats;

public sealed class Plant_LightRequirementColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Plant_LightRequirementColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null || plantProps.growMinGlow == 0f)
        {
            return "";
        }

        return plantProps.growMinGlow.ToStringPercent();
    }
}
