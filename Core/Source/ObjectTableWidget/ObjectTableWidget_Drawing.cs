using System;
using Stats.ColumnWorkers;
using Stats.MainTabWindow;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed partial class ObjectTableWidget<TObject>
{
    internal override void Draw(Rect rect, bool showSettingsMenu)
    {
        if (_guiAction != null)
        {
            _guiAction.Invoke();
            _guiAction = null;
        }

        if (Event.current.type == EventType.Layout)
        {
            RecalcLayout();
        }

        //if (showSettingsMenu)
        //{
        //    DrawColumnsTab(ref rect);
        //}

        Vector2 contentSize = _contentSize;
        float unpinnedRowsHeight = _unpinnedRowsHeight;
        float viewportWidth = unpinnedRowsHeight > 0f// Will scroll vertically
                ? rect.width - GenUI.ScrollBarWidth
                : rect.width;
        float viewportHeight = contentSize.x > rect.width// Will scroll horizontally
                ? rect.height - GenUI.ScrollBarWidth
                : rect.height;
        Vector2 viewportSize = new(viewportWidth, viewportHeight);
        Rect contentRect = new(Vector2.zero, Vector2.Max(contentSize, viewportSize));
        // Add empty space for more convenient vertical scrolling.
        contentRect.height += MathF.Min(unpinnedRowsHeight, viewportHeight);

        using (new GUIScrollContext(rect, ref _scrollPosition, contentRect)) { }

        Vector2 scrollPosition = _scrollPosition;
        Rect viewportRect = new(rect.position, viewportSize);

        float firstVisibleUnpinnedRowY = -scrollPosition.y % _rowHeight;
        int scrolledUnpinnedRowsCount = Mathf.FloorToInt(scrollPosition.y / _rowHeight);
        int viewportRowCapacity = Mathf.CeilToInt((viewportHeight - _rowHeight - _pinnedRowsHeight) / _rowHeight);
        int visibleUnpinnedRowsCount = Math.Min(UnpinnedRowsCount - scrolledUnpinnedRowsCount, viewportRowCapacity);
        if (visibleUnpinnedRowsCount < 0)
        {
            visibleUnpinnedRowsCount = 0;
        }

        int visibleUnpinnedRowsStart = _pinnedRowsCount + scrolledUnpinnedRowsCount;
        Span<int> visibleUnpinnedRows = stackalloc int[visibleUnpinnedRowsCount];
        for (int i = 0; i < visibleUnpinnedRowsCount; i++)
        {
            visibleUnpinnedRows[i] = _rows[visibleUnpinnedRowsStart + i];
        }

        Span<int> pinnedRows = stackalloc int[_pinnedRowsCount];
        for (int i = 0; i < _pinnedRowsCount; i++)
        {
            pinnedRows[i] = _rows[i];
        }

        // TODO: Columns span (do we can directly modify columns collection on pin/reorder and don't allocate delegates)

        // Background
        DrawBackground(viewportRect, visibleUnpinnedRows, firstVisibleUnpinnedRowY, visibleUnpinnedRowsStart);

        // Left part
        if (_pinnedColumnsCount > 0)
        {
            Rect leftPartRect = viewportRect.CutByX(_pinnedColumnsWidth);
            DrawPart(leftPartRect, PinnedColumns, leftPartRect.x, pinnedRows, visibleUnpinnedRows, firstVisibleUnpinnedRowY);
            // Separator line
            Widgets.Draw.VerticalLine(leftPartRect.xMax - 1f, leftPartRect.y, leftPartRect.height, MainTabWindowWidget.BorderLineColor);
        }

        // Right part
        ReadOnlyListSegment<Column> unpinnedColumns = UnpinnedColumns;
        if (unpinnedColumns.Length > 0)
        {
            using (new GUIClipContext(viewportRect))
            {
                Rect rightPartRect = viewportRect;
                rightPartRect.x = 0f;
                rightPartRect.y = 0f;
                DrawPart(rightPartRect, unpinnedColumns, -scrollPosition.x, pinnedRows, visibleUnpinnedRows, firstVisibleUnpinnedRowY);
            }

            // Horizontal scrolling
            if (Event.current.type == EventType.MouseDrag && _currentlyReorderedColumn == null && _currentlyResizedColumn == null)
            {
                // Register mouse-drag only below headers to not interfere with them.
                viewportRect.yMin += _rowHeight + _pinnedRowsHeight;
                if (Mouse.IsOver(viewportRect))
                {
                    _scrollPosition.x = Mathf.Max(_scrollPosition.x + Event.current.delta.x * -1f, 0f);
                }
            }
        }
    }

    private void DrawPart(
        Rect rect,
        ReadOnlyListSegment<Column> columns,
        float columnsOffsetX,
        Span<int> pinnedRows,
        Span<int> visibleUnpinnedRows,
        float visibleUnpinnedRowsOffsetY
    )
    {
        float rectXmax = rect.xMax;
        Rect columnRect = new(columnsOffsetX, rect.y, 0f, rect.height);
        int columnsCount = columns.Length;
        int lastColumnIndex = columnsCount - 1;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            columnRect.width = column.Width;
            float columnRectXmax = columnRect.xMax;

            if (columnRectXmax > 0f)
            {
                DrawColumn(columnRect, column, pinnedRows, visibleUnpinnedRows, visibleUnpinnedRowsOffsetY);
                if (i < lastColumnIndex)
                {
                    Widgets.Draw.VerticalLine(columnRectXmax - 1f, 0f, rect.height, _columnSeparatorLineColor);
                }
            }

            if (columnRectXmax >= rectXmax)
            {
                break;
            }

            columnRect.x = columnRectXmax;
        }
    }

    private void DrawColumn(Rect rect, Column column, Span<int> pinnedRows, Span<int> visibleUnpinnedRows, float visibleUnpinnedRowsOffsetY)
    {
        bool shouldDrawCells = column.Worker.ShouldDrawCells;

        Rect headerRect = rect.CutByY(_rowHeight);
        using (new GUIClipContext(headerRect))
        {
            headerRect.x = 0f;
            headerRect.y = 0f;
            column.DrawHeaderCell(headerRect);

            if (shouldDrawCells && pinnedRows.Length > 0)
            {
                Rect pinnedRowsRect = rect.CutByY(_pinnedRowsHeight);
                pinnedRowsRect.x = 0f;
                pinnedRowsRect.y = 0f;
                DrawColumnCells(pinnedRowsRect, column, pinnedRows);
            }
        }

        if (shouldDrawCells && visibleUnpinnedRows.Length > 0)
        {
            using (new GUIClipContext(rect))
            {
                rect.x = 0f;
                rect.y = visibleUnpinnedRowsOffsetY;
                DrawColumnCells(rect, column, visibleUnpinnedRows);
            }
        }
    }

    private void DrawColumnCells(Rect rect, Column column, Span<int> rows)
    {
        rect.height = _rowHeight;
        ColumnWorker<TObject> columnWorker = column.Worker;
        int rowsCount = rows.Length;
        for (int i = 0; i < rowsCount; i++)
        {
            try
            {
                columnWorker.DrawCell(rect, rows[i]);
            }
            catch
            {
                // TODO:
                // - Add tooltip with exception's message.
                // - Make the whole thing into a separate non-inlineable method.
                Verse.Widgets.DrawBoxSolid(rect, Color.red);
            }
            rect.y = rect.yMax;
        }
    }

    private void DrawBackground(Rect rect, Span<int> visibleUnpinnedRows, float visibleUnpinnedRowsOffsetY, int visibleUnpinnedRowsStart)
    {
        bool isRepaint = Event.current.type == EventType.Repaint;

        // Headers
        Rect headersRect = rect.CutByY(_rowHeight);
        if (isRepaint)
        {
            Verse.Widgets.DrawHighlight(headersRect);
            Verse.Widgets.DrawLineHorizontal(headersRect.x, headersRect.yMax - 1f, headersRect.width, MainTabWindowWidget.BorderLineColor);
        }

        // Pinned rows
        if (_pinnedRowsCount > 0)
        {
            Rect pinnedRowsRect = rect.CutByY(_pinnedRowsHeight);
            if (isRepaint)
            {
                Verse.Widgets.DrawStrongHighlight(pinnedRowsRect, _pinnedRowsBGColor);
                Verse.Widgets.DrawLineHorizontal(pinnedRowsRect.x, pinnedRowsRect.yMax - 1f, pinnedRowsRect.width, MainTabWindowWidget.BorderLineColor);
            }
        }

        // Unpinned rows
        if (visibleUnpinnedRows.Length > 0)
        {
            Rect rowRect = new(rect.x, rect.y, rect.width, _rowHeight + visibleUnpinnedRowsOffsetY);
            for (int i = 0; i < visibleUnpinnedRows.Length; i++)
            {
                //int row = visibleUnpinnedRows[i];
                if (isRepaint)
                {
                    if (Mouse.IsOver(rowRect))
                    {
                        Verse.Widgets.DrawHighlight(rowRect);
                    }
                    else if ((visibleUnpinnedRowsStart + i) % 2 == 0)
                    {
                        Verse.Widgets.DrawLightHighlight(rowRect);
                    }
                }

                rowRect.y = rowRect.yMax;
                rowRect.height = _rowHeight;
                if (rowRect.yMax > rect.yMax)
                {
                    rowRect.height -= rowRect.yMax - rect.yMax;
                }
            }
        }
    }

    //private void DrawColumnsTab(ref Rect rect)
    //{
    //    var columnsTabWidgetSize = ColumnsTabWidget.GetSize(rect.size);
    //    var columnsTabRect = rect.CutByX(columnsTabWidgetSize.x + GenUI.ScrollBarWidth);
    //    var columnsTabRectMax = new Rect(Vector2.zero, columnsTabWidgetSize);
    //    // Adds empty space for more convenient vertical scrolling.
    //    columnsTabRectMax.height += columnsTabRect.height;

    //    Verse.Widgets.BeginScrollView(columnsTabRect, ref ColumnsTabScrollPosition, columnsTabRectMax, true);
    //    ColumnsTabWidget.DrawIn(columnsTabRectMax);
    //    Verse.Widgets.EndScrollView();
    //    Widgets.Draw.VerticalLine(
    //        columnsTabRect.xMax,
    //        rect.y,
    //        rect.height,
    //        MainTabWindowWidget.BorderLineColor
    //    );
    //    rect.xMin += 1f;
    //}
}
