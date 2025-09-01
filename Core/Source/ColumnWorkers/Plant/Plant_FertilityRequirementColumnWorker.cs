namespace Stats;

public sealed class Plant_FertilityRequirementColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Plant_FertilityRequirementColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.fertilityMin > 0f)
        {
            return (100F * plantProps.fertilityMin).ToDecimal(1);
        }

        return 0m;
    }
}
