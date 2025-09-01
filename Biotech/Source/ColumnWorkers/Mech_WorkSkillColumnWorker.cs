namespace Stats.Compat.Biotech;

public sealed class Mech_WorkSkillColumnWorker : NumberColumnWorker<AbstractThing>
{
    public Mech_WorkSkillColumnWorker(ColumnDef columndef) : base(columndef)
    {
    }
    protected override decimal GetValue(AbstractThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return raceProps.mechFixedSkillLevel;
        }

        return 0m;
    }
}
