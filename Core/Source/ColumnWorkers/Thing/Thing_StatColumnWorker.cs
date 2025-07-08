using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public class Thing_StatColumnWorker : ColumnWorker<ThingAlike>
{
    private static readonly Regex NumberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private readonly Func<ThingAlike, decimal> GetNumber;
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;
    protected StatDef Stat { get; }
    private readonly StatValueExplanationType ExplanationType;
    public Thing_StatColumnWorker(StatColumnDef columnDef) : base(columnDef, ColumnCellStyle.Number)
    {
        Stat = columnDef.stat;
        ExplanationType = columnDef.statValueExplanationType;
        GetNumber = new Func<ThingAlike, decimal>(thing =>
        {
            var label = GetStatDrawEntryLabel(thing);

            if (label != null)
            {
                var match = NumberRegex.Match(label);

                if (match.Success)
                {
                    return decimal.Parse(match.Groups[1].Captures[0].Value);
                }
            }

            return 0m;
        }).Memoized();
    }
    protected virtual string? GetStatValueExplanation(ThingAlike thing)
    {
        var worker = Stat.Worker;
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);
        var statValue = worker.GetValue(statRequest);

        return ExplanationType switch
        {
            StatValueExplanationType.Full => worker.GetExplanationFull(statRequest, _ToStringNumberSense, statValue),
            StatValueExplanationType.Unfinalized => worker.GetExplanationUnfinalized(statRequest, _ToStringNumberSense),
            StatValueExplanationType.FinalizePart => worker.GetExplanationFinalizePart(statRequest, _ToStringNumberSense, statValue),
            _ => null,
        };
    }
    protected virtual string? GetStatDrawEntryLabel(ThingAlike thing)
    {
        var worker = Stat.Worker;
        var statRequest = StatRequest.For(thing.Def, thing.StuffDef);

        if (worker.ShouldShowFor(statRequest))
        {
            var statValue = worker.GetValue(statRequest);

            if (statValue != 0f)
            {
                return worker.GetStatDrawEntryLabel(Stat, statValue, _ToStringNumberSense, statRequest);
            }
        }

        return null;
    }
    public sealed override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var label = GetStatDrawEntryLabel(thing);

        if (label == null)
        {
            return null;
        }

        var widget = new Label(label);

        if (ExplanationType != StatValueExplanationType.None)
        {
            var tooltip = GetStatValueExplanation(thing);

            if (tooltip != null)
            {
                return widget.Tooltip(tooltip);
            }
        }

        return widget;
    }
    public sealed override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> _)
    {
        return Make.NumberFilter(GetNumber);
    }
    public sealed override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetNumber(thing1).CompareTo(GetNumber(thing2));
    }
}
