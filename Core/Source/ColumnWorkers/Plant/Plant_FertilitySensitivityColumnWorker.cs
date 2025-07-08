namespace Stats;

public sealed class Plant_FertilitySensitivityColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_FertilitySensitivityColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0\\%")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.fertilitySensitivity > 0f)
        {
            return (100f * plantProps.fertilitySensitivity).ToDecimal(0);
        }

        return 0m;
    }
}
