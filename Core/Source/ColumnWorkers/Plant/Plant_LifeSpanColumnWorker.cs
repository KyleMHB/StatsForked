namespace Stats;

public sealed class Plant_LifeSpanColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Plant_LifeSpanColumnWorker(ColumnDef columnDef) : base(columnDef, formatString: "0.0 d")
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.LifespanDays > 0f)
        {
            return plantProps.LifespanDays.ToDecimal(1);
        }

        return 0m;
    }
}
