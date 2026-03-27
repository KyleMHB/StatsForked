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
        int index = _columns.FindIndex(column => column.Def == columnDef);
        RemoveColumn(index);
    }

    private sealed class Column
    {
        public float Width;
        public readonly ColumnDef Def;
        public bool IsWidthSetManually;

        private readonly ColumnWorker<TObject> _worker;
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

            _worker = worker;
            Def = worker.Def;
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
            ColumnWorker<TObject> worker = _worker;
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
            ColumnType columnType = _worker.Type;
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
                if (_parent._reorderedColumn == this)
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
            ColumnWorker<TObject> worker = _worker;
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
            Event @event = Event.current;

            if (@event.type == EventType.MouseDown && Mouse.IsOver(rect))
            {
                HandleMouseDown();
            }
            else if (_isResized)
            {
                if (OriginalEventUtility.EventType == EventType.MouseDrag)// Do resize
                {
                    DoResize();
                }
                else if (@event.rawType == EventType.MouseUp || @event.shift == false)
                {
                    _isResized = false;
                }
            }
            else if (
                OriginalEventUtility.EventType == EventType.MouseDrag
                && _parent._reorderedColumn != null
                && _parent._reorderedColumn != this
                && mouseXIsInVisibleArea
                && rect.x < @event.mousePosition.x && @event.mousePosition.x < rect.xMax
            )
            {
                // These checks are here to ensure that DoReorder() will not be called in vain.
                // mouseXIsInVisibleArea check is made so that unpinned columns that
                // overlap with pinned columns due to scroll do not "steal" reordered column.
                DoReorder(rect, _parent._reorderedColumn);
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                _parent._reorderedColumn = null;
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandleMouseDown()
        {
            Event @event = Event.current;

            if (@event.IsLMB())
            {
                if (@event.control)
                {
                    HandlePin();
                }
                else if (@event.shift)
                {
                    // Reset size
                    if (@event.clickCount > 1)
                    {
                        IsWidthSetManually = false;
                    }
                    else // Resize start 
                    {
                        _isResized = true;
                        IsWidthSetManually = true;
                        //_resizeWidthOffset = rect.xMax - @event.mousePosition.x;
                    }
                }
                else // Reorder start
                {
                    _parent._reorderedColumn = this;
                }
            }
            else if (@event.IsRMB())
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
        private void DoReorder(Rect rect, Column reorderedColumn)
        {
            ObjectTable<TObject> parent = _parent;
            List<Column> columns = parent._columns;
            int leftColumnsCount = parent._leftColumnsCount;
            int reorderedColumnIndex = columns.IndexOf(reorderedColumn);
            int thisColumnIndex = columns.IndexOf(this);

            bool reorderedColumnIsPinned = reorderedColumnIndex < leftColumnsCount;
            bool thisColumnIsPinned = thisColumnIndex < leftColumnsCount;
            if (reorderedColumnIsPinned && thisColumnIsPinned == false)
            {
                parent._leftColumnsCount--;
            }
            else if (reorderedColumnIsPinned == false && thisColumnIsPinned)
            {
                parent._leftColumnsCount++;
            }

            float xMiddle = rect.x + rect.width / 2f;
            float mouseX = Event.current.mousePosition.x;
            if (rect.x < mouseX && mouseX < xMiddle)// Left
            {
                if (thisColumnIndex - 1 != reorderedColumnIndex)
                {
                    parent._beforeDraw = () => columns.MoveBeforeElemAt(reorderedColumnIndex, thisColumnIndex);
                }
            }
            // xMiddle < mouseX && mouseX < rect.xMax check is not necessary here
            // because we're checking if mouseX is between rect.x and rect.xMax before calling the method.
            // So if mouseX is not on the left, it is guaranteed to be on the right.
            else// Right
            {
                if (thisColumnIndex + 1 != reorderedColumnIndex)
                {
                    parent._beforeDraw = () => columns.MoveAfterElemAt(reorderedColumnIndex, thisColumnIndex);
                }
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePin()
        {
            ObjectTable<TObject> parent = _parent;
            int index = parent._columns.IndexOf(this);
            if (index > parent._leftColumnsCount - 1)
            {
                parent._beforeDraw = () => parent.PinColumn(index);
            }
            else
            {
                parent._beforeDraw = () => parent.UnpinColumn(index);
            }
        }

        public void RecalcWidth(List<int> rows)
        {
            Width = Mathf.Max(_titleWidgetWidth, _worker.GetWidth(rows)) + GUIStyles.TableCell.PadHor * 2f;
        }
    }
}
