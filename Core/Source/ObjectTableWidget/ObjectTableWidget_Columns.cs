using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableCells;
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
        public float Width;
        public readonly ColumnWorker<TObject> Worker;
        public bool IsWidthSetManually;
        public readonly TableCellStyleType CellStyle;

        private readonly Widget _titleWidget;
        private readonly float _titleWidgetWidth;
        private readonly TipSignal _tooltip;
        private readonly ObjectTableWidget<TObject> _parent;

        public Column(ColumnWorker<TObject> worker, TableWorker tableWorker, ObjectTableWidget<TObject> parent)
        {
            ColumnDef def = worker.Def;
            Widget titleWidget = def.Title;
            Vector2 titleWidgetSize = titleWidget.GetSize();
            TableCellDescriptor cellDescriptor = worker.GetCellDescriptor(tableWorker);

            Worker = worker;
            _titleWidget = titleWidget;
            _titleWidgetWidth = titleWidgetSize.x;
            _tooltip = $"<i>{def.LabelCap}</i>\n\n{def.Description}";
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

            _titleWidget.DrawIn(titleRect);

            Event currentEvent = Event.current;
            bool mouseIsOverRect = Mouse.IsOver(rect);

            if (currentEvent.type == EventType.Repaint && mouseIsOverRect)
            {
                GUI.DrawTexture(rect, TexUI.HighlightTex, ScaleMode.StretchToFill, true, 0f, Color.white, 0f, 0f);
            }

            TooltipHandler.TipRegion(rect, _tooltip);

            MouseoverSounds.DoRegion(rect);
            if (currentEvent.control && GUI.Button(rect, "", Verse.Widgets.EmptyStyle))
            {
                HandlePinning();
            }

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
                Verse.Widgets.DrawHighlightSelected(rect);
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
            Width = Mathf.Max(_titleWidgetWidth, Worker.GetWidth(rows)) + CellPadHor * 2f;
        }
    }
}
