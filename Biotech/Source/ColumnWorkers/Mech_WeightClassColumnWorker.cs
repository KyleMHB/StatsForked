using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_WeightClassColumnWorker : DefColumnWorker<ThingAlike, MechWeightClassDef>
{
    public Mech_WeightClassColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected override MechWeightClassDef GetValue(ThingAlike thing)
    {
        return thing.Def.race.mechWeightClass;
    }
}
