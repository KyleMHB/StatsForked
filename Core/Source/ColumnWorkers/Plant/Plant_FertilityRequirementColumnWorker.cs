namespace Stats;

public sealed class Plant_FertilityRequirementColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_FertilityRequirementColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.fertilityMin > 0f)
        {
            return (100F * plantProps.fertilityMin).ToDecimal(1);
        }

        return 0m;
    }
}
