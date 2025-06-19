using Verse;

namespace Stats;

public sealed class Plant_FertilitySensitivityColumnWorker : StatDrawEntryColumnWorker<ThingAlike>
{
    public Plant_FertilitySensitivityColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override string GetStatDrawEntryLabel(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps == null || plantProps.fertilitySensitivity == 0f)
        {
            return "";
        }

        return plantProps.fertilitySensitivity.ToStringPercent();
    }
}
