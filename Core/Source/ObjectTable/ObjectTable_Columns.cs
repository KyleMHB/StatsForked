using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.Extensions;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Widgets_Legacy;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
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
        public float Width;
        public readonly ColumnWorker<TObject> Worker;
        public bool IsWidthSetManually;
        public readonly TableCellStyleType CellStyle;

        private readonly Widget _titleWidget;
        private readonly float _titleWidgetWidth;
        private readonly TipSignal _tooltip;
        private readonly ObjectTable<TObject> _parent;

        public Column(ColumnWorker<TObject> worker, TableWorker tableWorker, ObjectTable<TObject> parent)
        {
            ColumnDef def = worker.Def;
            Widget titleWidget = def.TitleWidget;
            Vector2 titleWidgetSize = titleWidget.GetSize();
            TableCellDescriptor cellDescriptor = worker.GetCellDescriptor(tableWorker);

            Worker = worker;
            _titleWidget = titleWidget;
            _titleWidgetWidth = titleWidgetSize.x;
            _tooltip = $"<i>{def.LabelCap}</i>\n\n{def.description}";
            CellStyle = cellDescriptor.Style;
            _parent = parent;
        }

        public void DrawHeaderCell(Rect rect)
        {
            Rect titleRect = rect.ContractedByObjectTableCellPadding();
            float titleWidgetWidth = _titleWidgetWidth;
            float widthDiff = titleRect.width - titleWidgetWidth;
            titleRect.width = titleWidgetWidth;
            TableCellStyleType cellStyle = CellStyle;
            if (cellStyle == TableCellStyleType.Number)
            {
                titleRect.x += widthDiff;
            }
            else if (cellStyle == TableCellStyleType.Boolean)
            {
                titleRect.x += widthDiff / 2f;
            }

            _titleWidget.Draw(titleRect, Vector2.zero);

            Event currentEvent = Event.current;
            bool mouseIsOverRect = Mouse.IsOver(rect);

            // Manual resizing
            if (mouseIsOverRect && currentEvent.shift)
            {
                if (currentEvent.clickCount == 2)
                {
                    IsWidthSetManually = false;
                }
                else if (_parent._currentlyResizedColumn == null && currentEvent.type == EventType.MouseDown)
                {
                    _parent._currentlyResizedColumn = this;
                    IsWidthSetManually = true;
                }

                if (_parent._currentlyResizedColumn == this && currentEvent.type == EventType.MouseDrag)
                {
                    Width = Mathf.Max(Width + currentEvent.delta.x, _parent._rowHeight);
                }
            }

            // Reordering
            if (mouseIsOverRect && currentEvent.type == EventType.MouseDrag && _parent._currentlyResizedColumn == null && _parent._currentlyReorderedColumn == null)
            {
                _parent._currentlyReorderedColumn = this;
            }

            if (_parent._currentlyReorderedColumn == this && currentEvent.type == EventType.Repaint)
            {
                rect.HighlightSelected();
            }

            if (currentEvent.type == EventType.MouseDrag && _parent._currentlyReorderedColumn != null && _parent._currentlyReorderedColumn != this)
            {
                // TODO: Check if a column on the left/right is already _parent._currentlyReorderedColumn
                if (Mouse.IsOver(rect.LeftHalf()))
                {
                    _parent._guiAction = () =>
                    {
                        _parent._columns.Remove(_parent._currentlyReorderedColumn);
                        int thisColumnIndex = _parent._columns.IndexOf(this);
                        _parent._columns.Insert(thisColumnIndex, _parent._currentlyReorderedColumn);
                    };
                }
                else if (Mouse.IsOver(rect.RightHalf()))
                {
                    _parent._guiAction = () =>
                    {
                        _parent._columns.Remove(_parent._currentlyReorderedColumn);
                        int thisColumnIndex = _parent._columns.IndexOf(this);
                        _parent._columns.Insert(thisColumnIndex + 1, _parent._currentlyReorderedColumn);
                    };
                }
            }

            if (currentEvent.type == EventType.MouseUp)
            {
                _parent._currentlyResizedColumn = null;
                _parent._currentlyReorderedColumn = null;
            }

            // Pinning
            if (rect.ButtonGhostly() && currentEvent.control)
            {
                HandlePinning();
            }

            rect.Tip(_tooltip);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePinning()
        {
            int index = _parent._columns.IndexOf(this);
            if (index > _parent._pinnedColumnsCount - 1)
            {
                _parent._guiAction = () => _parent.PinColumn(index);
            }
            else
            {
                _parent._guiAction = () => _parent.UnpinColumn(index);
            }
        }

        public void RecalcWidth(List<int> rows)
        {
            Width = Mathf.Max(_titleWidgetWidth, Worker.GetWidth(rows)) + TableCellStyle.CellPadHor * 2f;
        }
    }
}
