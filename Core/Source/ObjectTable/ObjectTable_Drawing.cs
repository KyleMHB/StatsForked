using System;
using Stats.ColumnWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.Table;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    internal override void Draw(Rect rect)
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

        // Layout
        rect
            .CutTop(out Rect toolbarRect, GUIStyles.TableToolbar.Height)
            .TakeRest(out Rect tableRect);

        // Toolbar
        _toolbar.Draw(toolbarRect);

        //if (showSettingsMenu)
        //{
        //    DrawColumnsTab(ref rect);
        //}

        Rect viewportRect = tableRect;
        Rect contentRect = new(Vector2.zero, _contentSize);
        // Will scroll vertically
        if (_bottomRowsHeight > 0f)
        {
            viewportRect.width -= GenUI.ScrollBarWidth;
            // Add empty space for more convenient vertical scrolling.
            contentRect.height += viewportRect.height - HeadersRowHeight - _topRowsHeight;
        }
        // Will scroll horizontally
        if (contentRect.width > viewportRect.width)
        {
            viewportRect.height -= GenUI.ScrollBarWidth;
            contentRect.height -= GenUI.ScrollBarWidth;
        }

        using (new GUIScrollScope(tableRect, ref _scrollPosition, contentRect)) { }

        DrawVisibleContent(viewportRect);
    }

    private void DrawVisibleContent(Rect rect)
    {
        // O(1) scroll content culling.
        // Since all rows have constant height, we can calculate:
        // - From what row/y to start drawing rows.
        // - How many rows to draw.
        Vector2 scrollPosition = _scrollPosition;
        float firstVisibleBottomRowY = -scrollPosition.y % RowHeight;
        int scrolledBottomRowsCount = Mathf.FloorToInt(scrollPosition.y / RowHeight);
        int bottomRowsLeftToScroll = BottomRowsCount - scrolledBottomRowsCount;
        float bottomRowsRectHeight = rect.height - HeadersRowHeight - _topRowsHeight;
        int bottomRowsRectRowCapacity = Mathf.CeilToInt(bottomRowsRectHeight / RowHeight);
        int visibleBottomRowsCount = Math.Min(bottomRowsLeftToScroll, bottomRowsRectRowCapacity);
        // There was a bug where this value was evaluating to negative int which caused CTD when
        // int[] was being allocated on a stack below.
        // Although i couldn't reproduce it in recent testing, i'll leave this check just in case.
        // The math for this value is not very precise (lots of divisions, rounding and float values are involved).
        if (visibleBottomRowsCount < 0)
        {
            visibleBottomRowsCount = 0;
        }
        int topRowsCount = _topRowsCount;

        int visibleBottomRowsStart = topRowsCount + scrolledBottomRowsCount;
        Span<int> visibleBottomRows = stackalloc int[visibleBottomRowsCount];
        _rows.CopyTo(visibleBottomRows, visibleBottomRowsStart);

        Span<int> topRows = stackalloc int[topRowsCount];
        _rows.CopyTo(topRows);

        // Layout
        rect
            .CutLeft(out Rect leftColumnsRect, _leftColumnsWidth)
            .TakeRest(out Rect rightColumnsRect)
            .SkipTop(HeadersRowHeight)// Register mouse-drag only below headers to not interfere with them.
            .TakeRest(out Rect mouseDragScrollAreaRect);

        // Rows
        DrawRows(rect, visibleBottomRowsStart, visibleBottomRowsCount, firstVisibleBottomRowY);

        // Pinned columns
        if (_leftColumnsCount > 0)
        {
            DrawColumns(leftColumnsRect, LeftColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            // Separator line
            if (Event.current.IsRepaint())
            {
                leftColumnsRect.DrawBorderRight(FixedPartSeparatorLineColor);
            }
        }

        // Unpinned columns
        if (RightColumnsCount > 0)
        {
            using (new GUIClipScope(rightColumnsRect))
            {
                DrawColumns(rightColumnsRect with { xMin = -scrollPosition.x, y = 0f }, RightColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            }
        }

        // Horizontal scrolling
        if (Event.current.type == EventType.MouseDrag && Mouse.IsOver(mouseDragScrollAreaRect) && _currentlyReorderedColumn == null && _currentlyResizedColumn == null)
        {
            _scrollPosition.x = Mathf.Max(scrollPosition.x - Event.current.delta.x, 0f);
        }
    }

    private void DrawColumns(Rect rect, ReadOnlyListSegment<Column> columns, Span<int> topRows, Span<int> bottomRows, float bottomRowsY)
    {
        bool isRepaint = Event.current.IsRepaint();
        float rectXmax = rect.xMax;
        ref Rect columnRect = ref rect;
        int columnsCount = columns.Length;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            columnRect.width = column.Width;
            float columnRectXmax = columnRect.xMax;

            if (columnRectXmax > 0f)
            {
                DrawColumn(columnRect, column, topRows, bottomRows, bottomRowsY);
                if (isRepaint)
                {
                    columnRect.DrawBorderRight(ColumnSeparatorLineColor);
                }
            }

            if (columnRectXmax >= rectXmax)
            {
                break;
            }

            columnRect.x = columnRectXmax;
        }
    }

    private void DrawColumn(Rect rect, Column column, Span<int> topRows, Span<int> bottomRows, float bottomRowsY)
    {
        bool shouldDrawCellsNow = column.Worker.ShouldDrawCellsNow;

        // Layout
        rect
            .CutTop(out Rect topRect, HeadersRowHeight + _topRowsHeight)
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
            column.DrawHeaderCell(headerCellRect);

            // Pinned rows
            if (shouldDrawCellsNow && topRows.Length > 0)
            {
                DrawColumnCells(topRowsRect, column.Worker, topRows);
            }
        }

        // Unpinned rows
        if (shouldDrawCellsNow && bottomRows.Length > 0)
        {
            using (new GUIClipScope(bottomRowsRect))
            {
                DrawColumnCells(bottomRowsRect with { x = 0f, yMin = bottomRowsY }, column.Worker, bottomRows);
            }
        }
    }

    private void DrawColumnCells(Rect rect, ColumnWorker<TObject> columnWorker, Span<int> rows)
    {
        ref Rect cellRect = ref rect;
        cellRect.height = RowHeight;
        int rowsCount = rows.Length;
        for (int i = 0; i < rowsCount; i++)
        {
            try
            {
                columnWorker.DrawCell(cellRect, rows[i]);
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

    private void DrawRows(Rect rect, int bottomRowsStart, int bottomRowsCount, float bottomRowsY)
    {
        bool isRepaint = Event.current.type == EventType.Repaint;

        // Layout
        rect
            .CutTop(out Rect headersRowRect, HeadersRowHeight)
            .CutTop(out Rect topRowsRect, _topRowsHeight)
            .TakeRest(out Rect bottomRowsRect);

        // Headers row
        if (isRepaint)
        {
            headersRowRect
                .Highlight()
                .DrawBorderBottom(GUIStyles.MainTabWindow.BorderColor);
        }

        // Pinned rows
        int topRowsCount = _topRowsCount;
        if (topRowsCount > 0)
        {
            // Background
            if (isRepaint)
            {
                topRowsRect
                    .Fill(PinnedRowsBGColor)
                    .DrawBorderBottom(FixedPartSeparatorLineColor);
            }

            // Rows
            Rect rowRect = topRowsRect with { height = RowHeight };
            for (int i = 0; i < topRowsCount; i++)
            {
                DrawRow(rowRect, i);
                rowRect.y = rowRect.yMax;
            }
        }

        // Unpinned rows
        // 
        // This part uses manual clipping.
        // - No need to use GUI.BeginClip/EndClip.
        // - Top row's "click" event will never collide with bottom row.
        if (bottomRowsCount > 0)
        {
            float rectYmax = rect.yMax;
            float firstRowHeight = RowHeight + bottomRowsY;
            int bottomRowsEnd = bottomRowsCount + bottomRowsStart;// Exclusive
            Rect rowRect = bottomRowsRect with { height = firstRowHeight };
            for (int i = bottomRowsStart; i < bottomRowsEnd; i++)
            {
                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
                rowRect.height = RowHeight;

                if (rowRect.yMax > rectYmax)
                {
                    rowRect.yMax = rectYmax;
                }
            }
        }
    }

    private void DrawRow(Rect rect, int index)
    {
        bool mouseIsOverRect = Mouse.IsOver(rect);

        if (Event.current.IsRepaint())
        {
            if (mouseIsOverRect)
            {
                rect.Highlight();
            }
            else if (index % 2 == 0)
            {
                rect.HighlightLight();
            }
        }

        if (mouseIsOverRect && Event.current is { control: true, type: EventType.MouseDown })
        {
            HandleRowPin(index);
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
