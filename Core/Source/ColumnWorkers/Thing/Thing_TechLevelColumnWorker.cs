using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Thing_TechLevelColumnWorker : ColumnWorker<ThingAlike>
{
    public Thing_TechLevelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<TechLevel, string> GetTechLevelString =
    FunctionExtensions.Memoized((TechLevel techLevel) =>
    {
        return techLevel.ToStringHuman().CapitalizeFirst();
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        if (thing.Def.techLevel == TechLevel.Undefined)
        {
            return null;
        }

        return new Label(GetTechLevelString(thing.Def.techLevel));
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var options = tableRecords
            .Select(thing => thing.Def.techLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel)
            .Select<TechLevel, NTMFilterOption<TechLevel>>(
                techLevel => new(techLevel, GetTechLevelString(techLevel))
            );

        return Make.OTMFilter((ThingAlike thing) => thing.Def.techLevel, options);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return thing1.Def.techLevel.CompareTo(thing2.Def.techLevel);
    }
}
