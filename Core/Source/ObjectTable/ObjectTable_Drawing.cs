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

        // Toolbar
        Rect toolbarRect = rect.CutByY(GUIStyles.TableToolbar.Height);
        _toolbar.Draw(toolbarRect);

        //if (showSettingsMenu)
        //{
        //    DrawColumnsTab(ref rect);
        //}

        Rect viewportRect = rect;
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

        using (new GUIScrollScope(rect, ref _scrollPosition, contentRect)) { }

        DrawVisibleContent(viewportRect);
    }

    private void DrawVisibleContent(Rect rect)
    {
        Vector2 scrollPosition = _scrollPosition;
        float firstVisibleBottomRowY = -scrollPosition.y % RowHeight;
        int scrolledBottomRowsCount = Mathf.FloorToInt(scrollPosition.y / RowHeight);
        int viewportRowCapacity = Mathf.CeilToInt((rect.height - HeadersRowHeight - _topRowsHeight) / RowHeight);
        int visibleBottomRowsCount = Math.Min(BottomRowsCount - scrolledBottomRowsCount, viewportRowCapacity);
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
            .CutByX(out Rect leftPartRect, _leftColumnsWidth)
            .TakeRest(out Rect rightPartRect)
            .SkipY(HeadersRowHeight)// Register mouse-drag only below headers to not interfere with them.
            .TakeRest(out Rect mouseDragScrollAreaRect);

        // Background
        DrawBackground(rect, visibleBottomRowsStart, visibleBottomRowsCount, firstVisibleBottomRowY);

        // Left part
        int leftColumnsCount = _leftColumnsCount;
        if (leftColumnsCount > 0)
        {
            ReadOnlyListSegment<Column> leftColumns = new(_columns, 0, leftColumnsCount);

            DrawColumns(leftPartRect, leftColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            // Separator line
            if (Event.current.IsRepaint())
            {
                leftPartRect.DrawBorderRight(FixedPartSeparatorLineColor);
            }
        }

        // Right part
        int rightColumnsCount = _columns.Count - leftColumnsCount;
        if (rightColumnsCount > 0)
        {
            ReadOnlyListSegment<Column> rightColumns = new(_columns, leftColumnsCount, rightColumnsCount);

            using (new GUIClipScope(rightPartRect))
            {
                rightPartRect.xMin = -scrollPosition.x;
                rightPartRect.y = 0f;
                DrawColumns(rightPartRect, rightColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            }
        }

        // Horizontal scrolling
        if (Event.current.type == EventType.MouseDrag && Mouse.IsOver(mouseDragScrollAreaRect) && _currentlyReorderedColumn == null && _currentlyResizedColumn == null)
        {
            _scrollPosition.x = Mathf.Max(scrollPosition.x + Event.current.delta.x * -1f, 0f);
        }
    }

    private void DrawColumns(Rect rect, ReadOnlyListSegment<Column> columns, Span<int> topRows, Span<int> bottomRows, float bottomRowsY)
    {
        float rectXmax = rect.xMax;
        ref Rect columnRect = ref rect;
        int columnsCount = columns.Length;
        int lastColumnIndex = columnsCount - 1;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            columnRect.width = column.Width;
            float columnRectXmax = columnRect.xMax;

            if (columnRectXmax > 0f)
            {
                DrawColumn(columnRect, column, topRows, bottomRows, bottomRowsY);
                if (Event.current.IsRepaint() && i < lastColumnIndex)
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
            .CutByY(out Rect topRect, HeadersRowHeight + _topRowsHeight)
            .TakeRest(out Rect bottomRowsRect);

        // Header cell and pinned rows
        using (new GUIClipScope(topRect))
        {
            // Layout
            topRect
                .AtZero()
                .CutByY(out Rect headerCellRect, HeadersRowHeight)
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
                bottomRowsRect.x = 0f;
                bottomRowsRect.yMin = bottomRowsY;
                DrawColumnCells(bottomRowsRect, column.Worker, bottomRows);
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

    private void DrawBackground(Rect rect, int bottomRowsStart, int bottomRowsCount, float bottomRowsY)
    {
        bool isRepaint = Event.current.type == EventType.Repaint;

        // Layout
        rect
            .CutByY(out Rect headersRowRect, HeadersRowHeight)
            .CutByY(out Rect topRowsRect, _topRowsHeight)
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
            Rect rowRect = bottomRowsRect;
            rowRect.height = RowHeight + bottomRowsY;
            for (int i = 0; i < bottomRowsCount; i++)
            {
                int rowIndex = bottomRowsStart + i;
                DrawRow(rowRect, rowIndex);

                // TODO: I think this part can be optimized
                rowRect.y = rowRect.yMax;
                rowRect.height = RowHeight;
                if (rowRect.yMax > rect.yMax)
                {
                    rowRect.height -= rowRect.yMax - rect.yMax;
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
            if (index < _topRowsCount)
            {
                UnpinRow(index);
            }
            else
            {
                PinRow(index);
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
