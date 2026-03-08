using System.Text.RegularExpressions;
using RimWorld;
using Stats.FilterWidgets;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.BuildableDef;

public class StatColumnWorker(StatColumnDef columnDef) : ColumnWorker<DefBasedObject, StatColumnWorker.StatCell>
{
    public override ColumnDef Def => columnDef;

    private static readonly Regex _numberRegex = new(@"(-?[0-9]+\.?[0-9]*).*", RegexOptions.Compiled);
    private const ToStringNumberSense _ToStringNumberSense = ToStringNumberSense.Absolute;
    private readonly StatDef _stat = columnDef.stat;

    protected override StatCell MakeCell(DefBasedObject @object)
    {
        StatRequest statRequest;
        if (@object.Thing != null)
        {
            statRequest = StatRequest.For(@object.Thing);
        }
        else if (@object.Def is Verse.BuildableDef buildableDef)
        {
            statRequest = StatRequest.For(buildableDef, @object.StuffDef);
        }
        else
        {
            return default;
        }

        if (_stat.Worker.ShouldShowFor(statRequest))
        {
            float statValue = _stat.Worker.GetValue(statRequest);

            return new StatCell(statValue, statRequest, _stat, this);
        }

        return default;
    }

    protected override StatCell RefreshCell(StatCell cell, out bool wasStale)
    {
        float newStatValue = _stat.Worker.GetValue(cell.StatRequest);

        if (newStatValue != cell.ValueRaw)
        {
            if (_cellTooltip != null && _cellTooltipOwner == cell.StatRequest)
            {
                _cellTooltip = null;
            }

            wasStale = true;
            return new StatCell(newStatValue, cell.StatRequest, _stat, this);
        }

        wasStale = false;
        return cell;
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        FilterWidget valueFieldFilter = new NumberFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        TableCellFieldDescriptor valueField = new(Def.Title, valueFieldFilter, Compare);

        return new TableCellDescriptor(TableCellStyleType.Number, [valueField]);
    }

    public override void NotifyRowRemoved(int row)
    {
        _cellTooltipOwner = default;
        // TODO:
        //
        // Set _cellTooltip to null also?
        // Although cell removal code is already a bit heavier than i wanted it to be,
        // and i don't think a single string, that just hangs around in memory, is that big of an issue.

        base.NotifyRowRemoved(row);
    }

    // TODO:
    //
    // Tooltip may become stale despite the cell's value being fresh.
    // This happens when we do not refresh a cell because its value haven't changed.
    // But the way we got to this value may have changed, and tooltip displays that info.
    private StatRequest _cellTooltipOwner;
    private TipSignal? _cellTooltip;

    public readonly struct StatCell : ITableCell
    {
        public float Width { get; }
        public bool IsRefreshable => StatRequest.Thing != null;
        public readonly decimal Value;
        public readonly float ValueRaw;
        public readonly StatRequest StatRequest;

        private readonly StatDef? _stat;
        private readonly string? _text;
        private readonly StatColumnWorker _column;

        public StatCell(float statValue, StatRequest statRequest, StatDef stat, StatColumnWorker column)
        {
            _column = column;
            _stat = stat;
            StatRequest = statRequest;
            if (statValue != 0f)
            {
                ValueRaw = statValue;
                _text = stat.Worker.GetStatDrawEntryLabel(stat, statValue, _ToStringNumberSense, statRequest);
                Width = Text.CalcSize(_text).x;
                Match match = _numberRegex.Match(_text);
                if (match.Success)
                {
                    Value = decimal.Parse(match.Groups[1].Captures[0].Value);
                }
            }
        }

        public void Draw(Rect rect)
        {
            if (_text != null && Event.current.type == EventType.Repaint)
            {
                if (Mouse.IsOver(rect))
                {
                    if (_column._cellTooltip == null || _column._cellTooltipOwner != StatRequest)
                    {
                        _column._cellTooltip = _stat.Worker.GetExplanationFull(StatRequest, _ToStringNumberSense, ValueRaw);
                        _column._cellTooltipOwner = StatRequest;
                    }
                    TooltipHandler.TipRegion(rect, _column._cellTooltip.Value);
                }

                rect = rect.ContractedByObjectTableCellPadding();
                Verse.Widgets.Label(rect, _text);
            }
        }
    }
}
