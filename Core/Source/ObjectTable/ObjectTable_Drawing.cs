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
            contentRect.height += viewportRect.height - RowHeight - _topRowsHeight;
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
        int viewportRowCapacity = Mathf.CeilToInt((rect.height - RowHeight - _topRowsHeight) / RowHeight);
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

        // Background
        DrawRows(rect, visibleBottomRowsStart, visibleBottomRowsCount, firstVisibleBottomRowY);

        // Left part
        int leftColumnsCount = _leftColumnsCount;
        if (leftColumnsCount > 0)
        {
            Rect leftPartRect = rect.CutByX(_leftColumnsWidth);
            ReadOnlyListSegment<Column> leftColumns = new(_columns, 0, leftColumnsCount);
            DrawColumns(leftPartRect, leftColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            // Separator line
            if (Event.current.IsRepaint())
            {
                leftPartRect.DrawBorderRight(GUIStyles.MainTabWindow.BorderColor);
            }
        }

        // Right part
        int rightColumnsCount = _columns.Count - leftColumnsCount;
        if (rightColumnsCount > 0)
        {
            using (new GUIClipScope(rect))
            {
                ReadOnlyListSegment<Column> rightColumns = new(_columns, leftColumnsCount, rightColumnsCount);
                DrawColumns(rect with { xMin = -scrollPosition.x, y = 0f }, rightColumns, topRows, visibleBottomRows, firstVisibleBottomRowY);
            }

            // Horizontal scrolling
            if (Event.current.type == EventType.MouseDrag && _currentlyReorderedColumn == null && _currentlyResizedColumn == null)
            {
                // Register mouse-drag only below headers to not interfere with them.
                rect.yMin += RowHeight + _topRowsHeight;
                if (Mouse.IsOver(rect))
                {
                    _scrollPosition.x = Mathf.Max(scrollPosition.x + Event.current.delta.x * -1f, 0f);
                }
            }
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
        bool shouldDrawCells = column.Worker.ShouldDrawCells;

        Rect topRect = rect.CutByY(RowHeight + _topRowsHeight);
        ref Rect bottomRowsRect = ref rect;

        using (new GUIClipScope(topRect))
        {
            topRect.x = 0f;
            topRect.y = 0f;
            Rect headerCellRect = topRect.CutByY(RowHeight);
            ref Rect topRowsRect = ref topRect;

            column.DrawHeaderCell(headerCellRect);

            if (shouldDrawCells && topRows.Length > 0)
            {
                DrawColumnCells(topRowsRect, column.Worker, topRows);
            }
        }

        if (shouldDrawCells && bottomRows.Length > 0)
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

    private void DrawRows(Rect rect, int bottomRowsStart, int bottomRowsCount, float bottomRowsY)
    {
        bool isRepaint = Event.current.type == EventType.Repaint;

        // Layout
        Rect headersRowRect = rect.CutByY(RowHeight);
        Rect topRowsRect = rect.CutByY(_topRowsHeight);
        ref Rect bottomRowsRect = ref rect;

        // Headers
        if (isRepaint)
        {
            headersRowRect
                .Highlight()
                .DrawBorderBottom(GUIStyles.MainTabWindow.BorderColor);
        }

        // Pinned rows
        int pinnedRowsCount = _topRowsCount;
        if (pinnedRowsCount > 0)
        {
            if (isRepaint)
            {
                topRowsRect
                    .HighlightStrong(PinnedRowsBGColor)
                    .DrawBorderBottom(GUIStyles.MainTabWindow.BorderColor);
            }

            for (int i = 0; i < pinnedRowsCount; i++)
            {
                DrawRow(topRowsRect.CutByY(RowHeight), i);
            }
        }

        // Unpinned rows
        // 
        // This part uses manual clipping.
        // - No need to use GUI.BeginClip/EndClip.
        // - Top row's "click" event will never collide with bottom row.
        if (bottomRowsCount > 0)
        {
            ref Rect rowRect = ref bottomRowsRect;
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
