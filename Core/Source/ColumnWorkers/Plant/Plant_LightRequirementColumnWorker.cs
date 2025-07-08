namespace Stats;

public sealed class Plant_LightRequirementColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_LightRequirementColumnWorker(ColumnDef columDef) : base(columDef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.growMinGlow > 0f)
        {
            return (100f * plantProps.growMinGlow).ToDecimal(0);
        }

        return 0m;
    }
}
