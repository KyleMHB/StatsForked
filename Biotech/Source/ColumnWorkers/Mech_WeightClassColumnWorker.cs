using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Mech_WeightClassColumnWorker(ColumnDef columnDef) : DefColumnWorker<DefBasedObject, DefCell>
{
    public override ColumnDef Def => columnDef;

    protected override DefCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is ThingDef thingDef)
        {
            MechWeightClassDef? mechWeightClass = thingDef.race?.mechWeightClass;
            if (mechWeightClass != null)
            {
                return new DefCell(mechWeightClass);
            }
        }

        return default;
    }

    protected override IEnumerable<Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((IRefRecordsProvider<ThingDef>)tableWorker).Records
            .Select(thingDef => (Def?)thingDef.race?.mechWeightClass)
            .Distinct();
    }
}
