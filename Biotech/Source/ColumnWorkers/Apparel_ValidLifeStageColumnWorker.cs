using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Apparel_ValidLifeStageColumnWorker : ColumnWorker<ThingAlike, (HashSet<DevelopmentalStage> LifeStages, string? Text)>
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
    private static readonly Func<DevelopmentalStage, string> GetLifeStageString =
    FunctionExtensions.Memoized((DevelopmentalStage lifeStage) =>
    {
        return lifeStage.ToString().Translate().CapitalizeFirst().RawText;
    });
    protected override Cell GetCell(ThingAlike thing)
    {
        var lifeStages = GetValidLifeStages(thing.Def);

        if (lifeStages.Count > 0)
        {
            var stringBuilder = new StringBuilder();

            foreach (var lifeStage in lifeStages.OrderBy(lifeStage => lifeStage))
            {
                stringBuilder.AppendInNewLine(GetLifeStageString(lifeStage));
            }

            var text = stringBuilder.ToString();
            var widget = new Label(text);

            return new(widget, (lifeStages, text));
        }

        return new(null, ([], null));
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        var filterOptions = tableRecords
            .SelectMany(thing => GetValidLifeStages(thing.Def))
            .Distinct()
            .OrderBy(lifeStage => lifeStage)
            .Select(lifeStage => new NTMFilterOption<DevelopmentalStage>(lifeStage, GetLifeStageString(lifeStage)));

        yield return new(ColumnDef.Title, Make.MTMFilter((ThingAlike thing) => Cells[thing].Data.LifeStages, filterOptions));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Comparer<string?>.Default.Compare(
            Cells[thing1].Data.Text,
            Cells[thing2].Data.Text
        );
    }
}
