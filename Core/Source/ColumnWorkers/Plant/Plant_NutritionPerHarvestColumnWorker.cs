using RimWorld;

namespace Stats;

public sealed class Plant_NutritionPerHarvestColumnWorker : NumberColumnWorker<ThingAlike>
{
    public Plant_NutritionPerHarvestColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.00")
    {
    }
    protected override decimal GetValue(ThingAlike thing)
    {
        var plantProps = thing.Def.plant;

        if (plantProps?.harvestedThingDef != null)
        {
            var statRequest = StatRequest.For(plantProps.harvestedThingDef, null);

            if (StatDefOf.Nutrition.Worker.ShouldShowFor(statRequest))
            {
                var result = plantProps.harvestYield * StatDefOf.Nutrition.Worker.GetValue(statRequest);

                return result.ToDecimal(2);
            }
        }

        return 0m;
    }
}
