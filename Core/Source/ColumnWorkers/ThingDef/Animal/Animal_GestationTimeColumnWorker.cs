using RimWorld;
using Stats.ObjectTable.ColumnWorkers;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class Animal_GestationTimeColumnWorker : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public Animal_GestationTimeColumnWorker(ColumnDef columndef) : base(columndef, formatString: "0.0 d")
    {
    }
    protected override decimal GetCellValueSource(VirtualThing thing)
    {
        var raceProps = thing.Def.race;

        if (raceProps != null)
        {
            return AnimalProductionUtility.GestationDaysLitter(thing.Def).ToDecimal(1);
        }

        return 0m;
    }
}
