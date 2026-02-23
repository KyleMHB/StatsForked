using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Stats.ObjectTable.Cells;
using Stats.Widgets;
using UnityEngine;
using Verse;
using Verse.Sound;

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
        public readonly Vector2 HeaderCellSize;

        private readonly IColumnWorker<TObject> _worker;
        private readonly Widget _titleWidget;
        private readonly float _titleWidgetWidth;
        private readonly TipSignal _tooltip;
        private readonly CellStyleType _cellStyle;
        private readonly ObjectTableWidget<TObject> _parent;

        public Column(int cellIndex, IColumnWorker<TObject> worker, TableWorker tableWorker, ObjectTableWidget<TObject> parent)
        {
            ColumnDef def = worker.Def;
            Widget titleWidget = def.Title;
            Vector2 titleWidgetSize = titleWidget.GetSize();
            CellDescriptor cellDescriptor = worker.GetCellDescriptor(tableWorker);

            _worker = worker;
            CellIndex = cellIndex;
            _titleWidget = titleWidget;
            _titleWidgetWidth = titleWidgetSize.x;
            HeaderCellSize = titleWidgetSize + new Vector2(CellPadHor * 2f, CellPadVer * 2f);
            _tooltip = $"<i>{def.LabelCap}</i>\n\n{def.Description}";
            _cellStyle = cellDescriptor.Style;
            _parent = parent;
        }

        public Cell MakeCell(TObject @object) => _worker.MakeCell(@object);

        public void DrawHeaderCell(Rect rect, int index)
        {
            Rect titleRect = rect.ContractedBy(CellPadHor, CellPadVer);
            float titleWidgetWidth = _titleWidgetWidth;
            float widthDiff = titleRect.width - titleWidgetWidth;
            CellStyleType cellStyle = _cellStyle;

            titleRect.width = titleWidgetWidth;

            if (cellStyle == CellStyleType.Number)
            {
                titleRect.x += widthDiff;
            }
            else if (cellStyle == CellStyleType.Boolean)
            {
                titleRect.x += widthDiff / 2f;
            }

            _titleWidget.DrawIn(titleRect);

            Event currentEvent = Event.current;

            if (currentEvent.type == EventType.Repaint && Mouse.IsOver(rect))
            {
                GUI.DrawTexture(rect, TexUI.HighlightTex, ScaleMode.StretchToFill, true, 0f, Color.white, 0f, 0f);
            }

            TooltipHandler.TipRegion(rect, _tooltip);

            MouseoverSounds.DoRegion(rect);
            if (currentEvent.control && GUI.Button(rect, "", Verse.Widgets.EmptyStyle))
            {
                HandlePinning(index);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePinning(int index)
        {
            List<Column> pinnedColumns = _parent._pinnedColumns;

            if (index > pinnedColumns.Count - 1 || pinnedColumns[index] != this)
            {
                _parent._guiAction = () => _parent.PinColumn(index);
            }
            else
            {
                _parent._guiAction = () => _parent.UnpinColumn(index);
            }
        }

        public void ResizeTo(List<Row> rows)
        {
            int cellIndex = CellIndex;
            float width = HeaderCellSize.x;

            for (int i = 0; i < rows.Count; i++)
            {
                Row row = rows[i];
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
