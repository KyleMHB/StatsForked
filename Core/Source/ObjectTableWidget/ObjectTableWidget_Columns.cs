using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using Stats.Widgets;
using UnityEngine;
using Verse;
using Verse.Sound;

namespace Stats;

internal sealed partial class ObjectTableWidget<TObject>
{
    private void PinColumn(int index)
    {
        List<Column> columns = _columns;
        Column column = columns[index];
        columns.RemoveAt(index);
        columns.Insert(_pinnedColumnsCount, column);
        _pinnedColumnsCount++;
    }

    private void UnpinColumn(int index)
    {
        List<Column> columns = _columns;
        Column column = columns[index];
        columns.RemoveAt(index);
        _pinnedColumnsCount--;
        columns.Insert(_pinnedColumnsCount, column);
    }

    private sealed class Column
    {
        public int CellIndex;
        public float Width;
        public readonly Vector2 HeaderCellSize;

        private readonly ColumnWorker<TObject> _worker;
        private readonly Widget _titleWidget;
        private readonly float _titleWidgetWidth;
        private readonly TipSignal _tooltip;
        private readonly TableCellStyleType _cellStyle;
        private readonly ObjectTableWidget<TObject> _parent;

        public Column(int cellIndex, ColumnWorker<TObject> worker, TableWorker tableWorker, ObjectTableWidget<TObject> parent)
        {
            ColumnDef def = worker.Def;
            Widget titleWidget = def.Title;
            Vector2 titleWidgetSize = titleWidget.GetSize();
            TableCellDescriptor cellDescriptor = worker.GetCellDescriptor(tableWorker);

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
            titleRect.width = titleWidgetWidth;
            TableCellStyleType cellStyle = _cellStyle;
            if (cellStyle == TableCellStyleType.Number)
            {
                titleRect.x += widthDiff;
            }
            else if (cellStyle == TableCellStyleType.Boolean)
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
            if (index > _parent._pinnedColumnsCount - 1)
            {
                _parent._guiAction = () => _parent.PinColumn(index);
            }
            else
            {
                _parent._guiAction = () => _parent.UnpinColumn(index);
            }
        }
    }
}
