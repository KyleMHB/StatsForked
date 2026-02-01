using RimWorld;
using Stats.Objects.ThingDef;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.Objects.ThingDef.ColumnWorkers.Plant;

public sealed class Plant_RawNutritionPerDayColumnWorker : NumberColumnWorker<VirtualThing>
{
    public Plant_RawNutritionPerDayColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.000/d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps is { growDays: > 0f })
        {
            var nutrition = thing.Def.GetStatValuePerceived(StatDefOf.Nutrition);

            return (nutrition / plantProps.GetGrowDaysActual()).ToDecimal(3);
        }

        return 0m;
    }
}
