using System;
using System.Collections.Generic;
using System.Linq;
using Stats.FilterWidgets;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class SizeColumnWorker(ColumnDef columnDef) : ColumnWorker<DefBasedObject, SizeColumnWorker.SizeCell>
{
    public override ColumnDef Def => columnDef;

    protected override SizeCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            IntVec2 size = GetSize(thingDef);

            return new SizeCell(size);
        }

        return default;
    }

    private static IntVec2 GetSize(Verse.ThingDef thingDef)
    {
        IntVec2 size = thingDef.size;

        // Because 4x5=5x4.
        return new IntVec2(Math.Max(size.x, size.z), Math.Min(size.x, size.z));
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<decimal>> valueFieldFilterOptions = ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(GetSize)
            .Distinct()
            .OrderBy(size => size.Area)
            .Select(size => new NTMFilterOption<decimal>(size.Area, size.ToStringCross()));
        FilterWidget valueFieldFilter = new OTMFilter<decimal>((int row) => this[row].Area, valueFieldFilterOptions);
        int Compare(int row1, int row2) => this[row1].Area.CompareTo(this[row2].Area);
        TableCellFieldDescriptor valueField = new(Def.Title, valueFieldFilter, Compare);

        return new TableCellDescriptor(TableCellStyleType.Number, [valueField]);
    }

    public readonly struct SizeCell : ITableCell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly decimal Area;

        private readonly string? _text;

        public SizeCell(IntVec2 size)
        {
            Area = size.Area;
            if (Area != 0m)
            {
                _text = size.ToStringCross();
                Width = Text.CalcSize(_text).x;
            }
        }

        public void Draw(Rect rect)
        {
            if (_text != null && Event.current.type == EventType.Repaint)
            {
                rect = rect.ContractedByObjectTableCellPadding();
                Widgets.Draw.Label(rect, _text, TableCellStyle.Number);
            }
        }
    }
}
