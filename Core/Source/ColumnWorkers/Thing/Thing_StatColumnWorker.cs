using System.Collections.Generic;
using System.Text.RegularExpressions;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public class Thing_StatColumnWorker : ColumnWorker<ThingAlike, decimal>
{
    public static readonly Regex NumberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;
    protected StatDef Stat { get; }
    private readonly StatValueExplanationType ExplanationType;
    public Thing_StatColumnWorker(StatColumnDef columnDef) : base(columnDef, ColumnCellStyle.Number)
    {
        Stat = columnDef.stat;
        ExplanationType = columnDef.statValueExplanationType;
    }
    protected virtual string? GetStatDrawEntryLabel(StatDef stat, float value, ToStringNumberSense numberSense, StatRequest statRequest)
    {
        return stat.Worker.GetStatDrawEntryLabel(stat, value, _ToStringNumberSense, statRequest);
    }
    protected override Cell GetCell(ThingAlike thing)
    {
        var statWorker = Stat.Worker;
        var statRequest = thing.Thing != null
            ? StatRequest.For(thing.Thing)
            : StatRequest.For(thing.Def, thing.StuffDef);

        if (statWorker.ShouldShowFor(statRequest))
        {
            var statValue = statWorker.GetValue(statRequest);

            if (statValue != 0f)
            {
                var cellText = GetStatDrawEntryLabel(Stat, statValue, _ToStringNumberSense, statRequest);

                if (cellText != null)
                {
                    var displayedValue = 0m;
                    var match = NumberRegex.Match(cellText);

                    if (match.Success)
                    {
                        displayedValue = decimal.Parse(match.Groups[1].Captures[0].Value);
                    }

                    var tooltip = ExplanationType switch
                    {
                        StatValueExplanationType.Full => statWorker.GetExplanationFull(statRequest, _ToStringNumberSense, statValue),
                        StatValueExplanationType.Unfinalized => statWorker.GetExplanationUnfinalized(statRequest, _ToStringNumberSense),
                        StatValueExplanationType.FinalizePart => statWorker.GetExplanationFinalizePart(statRequest, _ToStringNumberSense, statValue),
                        _ => null,
                    };

                    Widget widget = new Label(cellText)
                    .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

                    if (tooltip != null)
                    {
                        widget = widget.Tooltip(tooltip);
                    }

                    return new(widget, displayedValue);
                }
            }
        }

        return new(null, 0m);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> _)
    {
        yield return new(ColumnDef.Title, Make.NumberFilter((ThingAlike thing) => Cells[thing].Data));
    }
    public sealed override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Cells[thing1].Data.CompareTo(Cells[thing2].Data);
    }
    public override bool Refresh()
    {
        if (Stat.immutable)
        {
            return false;
        }

        var result = false;

        foreach (var (thing, cell) in Cells)
        {
            if (thing.Thing != null)
            {
                // TODO
            }
        }

        return result;
    }
}
