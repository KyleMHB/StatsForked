using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.Table;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private void PinColumn(int index)
    {
        List<Column> columns = _columns;
        Column column = columns[index];
        columns.RemoveAt(index);
        columns.Insert(_leftColumnsCount, column);
        _leftColumnsCount++;
    }

    private void UnpinColumn(int index)
    {
        List<Column> columns = _columns;
        Column column = columns[index];
        columns.RemoveAt(index);
        _leftColumnsCount--;
        columns.Insert(_leftColumnsCount, column);
    }

    private void AddColumn(ColumnDef columnDef)
    {
        Type workerClass = columnDef.workerClass;
        ColumnWorker<TObject> columnWorker = (ColumnWorker<TObject>)Activator.CreateInstance(workerClass, columnDef);
        Column column = new(columnWorker, _tableWorker, this);
        _columns.Add(column);
        columnWorker.NotifyRowAdded(_objects);
        _toolbar.NotifyColumnAdded(column);
    }

    private void RemoveColumn(int index)
    {
        if (index < _leftColumnsCount)
        {
            _leftColumnsCount--;
        }

        _toolbar.NotifyColumnRemoved(_columns[index]);
        _columns.RemoveAt(index);
    }

    private void RemoveColumn(Column column)
    {
        int index = _columns.IndexOf(column);
        RemoveColumn(index);
    }

    private void RemoveColumn(ColumnDef columnDef)
    {
        int index = _columns.FindIndex(column => column.Worker.Def == columnDef);
        RemoveColumn(index);
    }

    private sealed class Column
    {
        public float Width;
        public readonly ColumnWorker<TObject> Worker;
        public bool IsWidthSetManually;

        private readonly Widget _titleWidget;
        private readonly float _titleWidgetWidth;
        private readonly TipSignal _tooltip;
        private readonly ObjectTable<TObject> _parent;
        private readonly FloatMenu _menu;
        private bool _isResized;
        //private float _resizeWidthOffset;

        public Column(ColumnWorker<TObject> worker, TableWorker tableWorker, ObjectTable<TObject> parent)
        {
            ColumnDef def = worker.Def;
            Widget titleWidget = def.TitleWidget;
            Vector2 titleWidgetSize = titleWidget.Size;

            Worker = worker;
            _titleWidget = titleWidget;
            _titleWidgetWidth = titleWidgetSize.x;
            _tooltip = $"<i>{def.LabelCap}</i>\n\n{def.description}";
            _parent = parent;
            _menu = new FloatMenu([
                new FloatMenuOption("Remove", () => parent.RemoveColumn(this))
            ]);
        }

        public void Draw(Rect rect, Span<int> topRows, Span<int> bottomRows, float bottomRowsY, bool mouseXIsInVisibleArea)
        {
            ColumnWorker<TObject> worker = Worker;
            bool shouldDrawCellsNow = worker.ShouldDrawCellsNow;

            // Layout
            rect
                .CutTop(out Rect topRect, HeadersRowHeight + _parent._topRowsHeight)
                .TakeRest(out Rect bottomRowsRect);

            // Header cell and pinned rows
            using (new GUIClipScope(topRect))
            {
                // Layout
                topRect
                    .AtZero()
                    .CutTop(out Rect headerCellRect, HeadersRowHeight)
                    .TakeRest(out Rect topRowsRect);

                // Header cell
                DrawHeaderCell(headerCellRect, mouseXIsInVisibleArea);

                // Pinned rows
                if (shouldDrawCellsNow && topRows.Length > 0)
                {
                    DrawCells(topRowsRect, topRows);
                }
            }

            // Unpinned rows
            if (shouldDrawCellsNow && bottomRows.Length > 0)
            {
                using (new GUIClipScope(bottomRowsRect, new Vector2(0f, bottomRowsY)))
                {
                    DrawCells(bottomRowsRect with { x = 0f, y = 0f }, bottomRows);
                }
            }
        }

        private void DrawHeaderCell(Rect rect, bool mouseXIsInVisibleArea)
        {
            ColumnType columnType = Worker.Type;
            Rect titleRect = rect.ContractedByObjectTableCellPadding();
            if (columnType == ColumnType.Number)
            {
                titleRect.CutRight(out titleRect, _titleWidgetWidth);
            }
            else if (columnType == ColumnType.Boolean)
            {
                titleRect.CutMidX(out titleRect, _titleWidgetWidth);
            }

            if (Event.current.IsRepaint())
            {
                if (_parent._currentlyReorderedColumn == this)
                {
                    rect.HighlightSelected();
                }

                _titleWidget.Draw(titleRect);
            }
            else if (Event.current.type != EventType.Layout)
            {
                HandleMouseEvents(rect, mouseXIsInVisibleArea);
            }

            rect.ButtonGhostly();
            rect.Tip(_tooltip);
        }

        private void DrawCells(Rect rect, Span<int> rows)
        {
            ColumnWorker<TObject> worker = Worker;
            ref Rect cellRect = ref rect;
            cellRect.height = RowHeight;
            int rowsCount = rows.Length;
            for (int i = 0; i < rowsCount; i++)
            {
                try
                {
                    worker.DrawCell(cellRect, rows[i]);
                }
                catch
                {
                    // TODO:
                    // - Add tooltip with exception's message.
                    // - Make the whole thing into a separate non-inlineable method.
                    cellRect.Fill(Color.red);
                }
                cellRect.y = cellRect.yMax;
            }
        }

        private void HandleMouseEvents(Rect rect, bool mouseXIsInVisibleArea)
        {
            if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
            {
                HandleMouseDown();
            }
            else if (_isResized)
            {
                if (OriginalEventUtility.EventType == EventType.MouseDrag)// Do resize
                {
                    DoResize();
                }
                else if (Event.current.rawType == EventType.MouseUp || Event.current.shift == false)
                {
                    _isResized = false;
                }
            }
            else if (
                // This check makes it so that unpinned columns that
                // overlap with pinned columns due to scroll
                // do not "steal" reordered column.
                mouseXIsInVisibleArea
                && _parent._guiAction == null
                && OriginalEventUtility.EventType == EventType.MouseDrag
                // Check if some (other) column is being reordered.
                && _parent._currentlyReorderedColumn != null
                && _parent._currentlyReorderedColumn != this
            )
            {
                DoReorder(rect);
            }
            else if (Event.current.rawType == EventType.MouseUp)
            {
                _parent._currentlyReorderedColumn = null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandleMouseDown()
        {
            if (Event.current.IsLMB())
            {
                if (Event.current.control)
                {
                    HandlePin();
                }
                else if (Event.current.shift)
                {
                    // Reset size
                    if (Event.current.clickCount > 1)
                    {
                        IsWidthSetManually = false;
                    }
                    else // Resize start 
                    {
                        _isResized = true;
                        IsWidthSetManually = true;
                        //_resizeWidthOffset = rect.xMax - Event.current.mousePosition.x;
                    }
                }
                else // Reorder start
                {
                    _parent._currentlyReorderedColumn = this;
                }
            }
            else if (Event.current.IsRMB())
            {
                _menu.Open();
            }
        }

        private void DoResize()
        {
            //Width = Event.current.mousePosition.x + _resizeWidthOffset;
            // Simply setting column's width to current mouse position (+starting offset)
            // feels more "snappy", but has a bug:
            // When the table is scrolled all the way to the right and we start reducing
            // the width of a column, it starts pulling its left side to the right, which
            // results in double width reduction. Using delta.x instead, feels less responsive,
            // but is more reliable.
            Width += Event.current.delta.x;
            if (Width < RowHeight)
            {
                Width = RowHeight;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void DoReorder(Rect rect)
        {
            float mouseX = Event.current.mousePosition.x;
            float xMiddle = rect.x + rect.width / 2f;

            if (rect.x < mouseX && mouseX < xMiddle)
            {
                _parent._guiAction = () =>
                {
                    int reorderedColumnIndex = _parent._columns.IndexOf(_parent._currentlyReorderedColumn);
                    bool reorderedColumnIsPinned = reorderedColumnIndex < _parent._leftColumnsCount;
                    _parent._columns.RemoveAt(reorderedColumnIndex);
                    if (reorderedColumnIsPinned)
                    {
                        _parent._leftColumnsCount--;
                    }

                    int thisColumnIndex = _parent._columns.IndexOf(this);
                    bool thisColumnIsPinned = thisColumnIndex < _parent._leftColumnsCount;
                    _parent._columns.Insert(thisColumnIndex, _parent._currentlyReorderedColumn);
                    if (thisColumnIsPinned)
                    {
                        _parent._leftColumnsCount++;
                    }
                };
            }
            else if (xMiddle < mouseX && mouseX < rect.xMax)
            {
                _parent._guiAction = () =>
                {
                    int reorderedColumnIndex = _parent._columns.IndexOf(_parent._currentlyReorderedColumn);
                    bool reorderedColumnIsPinned = reorderedColumnIndex < _parent._leftColumnsCount;
                    _parent._columns.RemoveAt(reorderedColumnIndex);
                    if (reorderedColumnIsPinned)
                    {
                        _parent._leftColumnsCount--;
                    }

                    int thisColumnIndex = _parent._columns.IndexOf(this);
                    bool thisColumnIsPinned = thisColumnIndex < _parent._leftColumnsCount;
                    _parent._columns.Insert(thisColumnIndex + 1, _parent._currentlyReorderedColumn);
                    if (thisColumnIsPinned)
                    {
                        _parent._leftColumnsCount++;
                    }
                };
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePin()
        {
            int index = _parent._columns.IndexOf(this);
            if (index > _parent._leftColumnsCount - 1)
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
            Width = Mathf.Max(_titleWidgetWidth, Worker.GetWidth(rows)) + GUIStyles.TableCell.PadHor * 2f;
        }
    }
}
