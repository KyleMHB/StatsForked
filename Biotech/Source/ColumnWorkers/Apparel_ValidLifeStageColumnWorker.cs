using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Apparel_ValidLifeStageColumnWorker : ColumnWorker<ThingAlike>
{
    public Apparel_ValidLifeStageColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    private static readonly Func<ThingDef, HashSet<DevelopmentalStage>> GetValidLifeStages =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        var lifeStages = new HashSet<DevelopmentalStage>();

        if (thingDef.apparel == null)
        {
            return lifeStages;
        }

        var enumerator = thingDef.apparel.developmentalStageFilter.Enumerate().GetEnumerator();

        while (enumerator.MoveNext())
        {
            lifeStages.Add(enumerator.Current);
        }

        return lifeStages;
    });
    private static readonly Func<ThingDef, string> GetLifeStageLabels =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        var lifeStages = GetValidLifeStages(thingDef);

        if (lifeStages.Count == 0)
        {
            return "";
        }

        var stringBuilder = new StringBuilder();

        foreach (var lifeStage in lifeStages.OrderBy(lifeStage => lifeStage))
        {
            stringBuilder.AppendInNewLine(GetLifeStageString(lifeStage));
        }

        return stringBuilder.ToString();
    });
    private static readonly Func<DevelopmentalStage, string> GetLifeStageString =
    FunctionExtensions.Memoized((DevelopmentalStage lifeStage) =>
    {
        return lifeStage.ToString().Translate().CapitalizeFirst().RawText;
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var lifeStageLabels = GetLifeStageLabels(thing.Def);

        if (lifeStageLabels.Length == 0)
        {
            return null;
        }

        return new Label(lifeStageLabels);
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        var filterOptions = tableRecords
            .SelectMany(thing => GetValidLifeStages(thing.Def))
            .Distinct()
            .OrderBy(lifeStage => lifeStage)
            .Select(lifeStage => new NTMFilterOption<DevelopmentalStage>(lifeStage, GetLifeStageString(lifeStage)));

        return Make.MTMFilter((ThingAlike thing) => GetValidLifeStages(thing.Def), filterOptions);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetLifeStageLabels(thing1.Def).CompareTo(GetLifeStageLabels(thing2.Def));
    }
}
