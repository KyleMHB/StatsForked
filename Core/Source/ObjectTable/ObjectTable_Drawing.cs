using System;
using Stats.ColumnWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    internal override void Draw(Rect rect)
    {
        bool isRepaint = Event.current.IsRepaint();

        if (_guiAction != null)
        {
            _guiAction.Invoke();
            _guiAction = null;
        }

        if (Event.current.type == EventType.Layout)
        {
            RecalcLayout();
        }

        // Toolbar
        Rect toolbarRect = rect.CutByY(30f);
        if (isRepaint)
        {
            toolbarRect
                .HighlightLight()
                .DrawBorderBottom(MainTabWindow.BorderColor);
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
        float viewportHeight = contentSize.x > viewportWidth// Will scroll horizontally
                ? rect.height - GenUI.ScrollBarWidth
                : rect.height;
        Vector2 viewportSize = new(viewportWidth, viewportHeight);
        Rect contentRect = new(Vector2.zero, Vector2.Max(contentSize, viewportSize));
        // Add empty space for more convenient vertical scrolling.
        contentRect.height += MathF.Min(unpinnedRowsHeight, viewportHeight);

        using (new GUIScrollScope(rect, ref _scrollPosition, contentRect)) { }

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
        _rows.CopyTo(visibleUnpinnedRows, visibleUnpinnedRowsStart);

        Span<int> pinnedRows = stackalloc int[_pinnedRowsCount];
        _rows.CopyTo(pinnedRows);

        // Background
        DrawBackground(viewportRect, visibleUnpinnedRows, firstVisibleUnpinnedRowY, visibleUnpinnedRowsStart);

        // Left part
        int pinnedColumnsCount = _pinnedColumnsCount;
        if (pinnedColumnsCount > 0)
        {
            Rect leftPartRect = viewportRect.CutByX(_pinnedColumnsWidth);
            ReadOnlyListSegment<Column> pinnedColumns = new(_columns, 0, pinnedColumnsCount);
            DrawPart(leftPartRect, pinnedColumns, leftPartRect.x, pinnedRows, visibleUnpinnedRows, firstVisibleUnpinnedRowY);
            // Separator line
            if (isRepaint) leftPartRect.DrawBorderRight(MainTabWindow.BorderColor);
        }

        // Right part
        int unpinnedColumnsCount = _columns.Count - pinnedColumnsCount;
        if (unpinnedColumnsCount > 0)
        {
            using (new GUIClipScope(viewportRect))
            {
                Rect rightPartRect = viewportRect with { x = 0f, y = 0f };
                ReadOnlyListSegment<Column> unpinnedColumns = new(_columns, pinnedColumnsCount, unpinnedColumnsCount);
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
                if (Event.current.IsRepaint() && i < lastColumnIndex)
                {
                    columnRect.DrawBorderRight(_columnSeparatorLineColor);
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
        using (new GUIClipScope(headerRect))
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
            using (new GUIClipScope(rect))
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
                rect.Fill(Color.red);
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
            headersRect
                .Highlight()
                .DrawBorderBottom(MainTabWindow.BorderColor);
        }

        // Pinned rows
        if (_pinnedRowsCount > 0)
        {
            Rect pinnedRowsRect = rect.CutByY(_pinnedRowsHeight);
            if (isRepaint)
            {
                pinnedRowsRect
                    .HighlightStrong(_pinnedRowsBGColor)
                    .DrawBorderBottom(MainTabWindow.BorderColor);
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
                        rowRect.Highlight();
                    }
                    else if ((visibleUnpinnedRowsStart + i) % 2 == 0)
                    {
                        rowRect.HighlightLight();
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
