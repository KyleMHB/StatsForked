using System.Collections.Generic;
using RimWorld;
using Stats.ObjectTable.Cells;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    private void PinColumn(int index)
    {
        List<Column> unpinnedColumns = _unpinnedColumns;

        _pinnedColumns.Add(unpinnedColumns[index]);
        unpinnedColumns.RemoveAt(index);
    }

    private void UnpinColumn(int index)
    {
        List<Column> pinnedColumns = _pinnedColumns;

        _unpinnedColumns.Insert(0, pinnedColumns[index]);
        pinnedColumns.RemoveAt(index);
    }

    private sealed class Column
    {
        public int CellIndex;
        public float Width;
        //public Widget Title => _def.Title;
        //public readonly TipSignal Tooltip;
        //public readonly CellDescriptor CellDescriptor;
        //public bool HasActiveFilters => CellDescriptor.Fields.Any(@field => @field.FilterWidget.IsActive);

        private readonly ColumnDef _def;
        private readonly IColumnWorker<TObject> _worker;
        //private readonly CellStyleType _cellStyle;

        public Column(int index, ColumnDef def, IColumnWorker<TObject> worker/*, TableWorker tableWorker*/)
        {
            _def = def;
            _worker = worker;
            Tooltip = $"<i>{def.LabelCap}</i>\n\n{def.Description}";
            //CellDescriptor cellDescriptor = worker.GetCellDescriptor(tableWorker);
            CellIndex = index;
        }

        public Cell MakeCell(TObject @object) => _worker.MakeCell(@object);

        public Widget MakeHeaderCell()
        {
            Widget columnTitle = Title;

            if (_cellStyle == CellStyleType.Number)
            {
                columnTitle = new SingleElementContainer(columnTitle.PaddingRel(1f, 0f, 0f, 0f));
            }
            else if (_cellStyle == CellStyleType.Boolean)
            {
                columnTitle = new SingleElementContainer(columnTitle.PaddingRel(0.5f, 0f));
            }

            return columnTitle
                .TextAnchor((TextAnchor)_cellStyle)
                .PaddingAbs(CellPadHor, CellPadVer)
                .ToButtonGhostly(() =>
                {
                    if (Event.current.control)
                    {
                        IsPinned = !IsPinned;
                    }
                })
                .Tooltip(Tooltip);
        }

        public void ResizeTo(List<Row> rows)
        {
            int cellIndex = CellIndex;
            // TODO: We actually need to start with header cell width.
            float width = 0f;

            foreach (Row row in rows)
            {
                Cell cell = row.Cells[cellIndex];
                float cellWidth = cell.Size.x;

                if (width < cellWidth)
                {
                    width = cellWidth;
                }
            }

            Width = width;
        }
    }
}
