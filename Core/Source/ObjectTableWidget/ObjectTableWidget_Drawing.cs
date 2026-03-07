using System;
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
        contentRect.height += unpinnedRowsHeight;

        using (new GUIScrollContext(rect, ref _scrollPosition, contentRect))
        {
            Vector2 scrollPosition = _scrollPosition;
            Rect viewportRect = new(scrollPosition, viewportSize);

            float visibleUnpinnedRowsOffsetY = scrollPosition.y % _rowHeight;
            int scrolledUnpinnedRowsCount = Mathf.FloorToInt(scrollPosition.y / _rowHeight);
            int viewportRowCapacity = Mathf.CeilToInt(viewportHeight / _rowHeight);
            int visibleUnpinnedRowsCount = Math.Min(_rows.Count - _pinnedRowsCount - scrolledUnpinnedRowsCount, viewportRowCapacity);
            ReadOnlyListSegment<int> visibleUnpinnedRows = UnpinnedRows.Slice(scrolledUnpinnedRowsCount, visibleUnpinnedRowsCount);

            // Background
            DrawBackground(viewportRect, visibleUnpinnedRows, visibleUnpinnedRowsOffsetY);

            // Left part
            int pinnedColumnsCount = _pinnedColumnsCount;
            if (pinnedColumnsCount > 0)
            {
                ReadOnlyListSegment<Column> pinnedColumns = PinnedColumns;
                float pinnedColumnsWidth = _pinnedColumnsWidth;
                Rect leftPartRect = viewportRect.CutByX(pinnedColumnsWidth);
                DrawPart(leftPartRect, pinnedColumns, 0f, visibleUnpinnedRows, visibleUnpinnedRowsOffsetY, false);
                // Separator line
                Widgets.Draw.VerticalLine(leftPartRect.xMax - 1f, leftPartRect.y, rect.height, MainTabWindowWidget.BorderLineColor);
            }

            // Right part
            ReadOnlyListSegment<Column> unpinnedColumns = UnpinnedColumns;
            if (unpinnedColumns.Length > 0)
            {
                DrawPart(viewportRect, unpinnedColumns, scrollPosition.x, visibleUnpinnedRows, visibleUnpinnedRowsOffsetY, true);
            }
        }
    }

    private void DrawPart(
        Rect rect,
        ReadOnlyListSegment<Column> columns,
        float columnsOffsetX,
        ReadOnlyListSegment<int> visibleUnpinnedRows,
        float visibleUnpinnedRowsOffsetY,
        bool doHorScroll
    )
    {
        using (new GUIClipContext(rect))
        {
            float viewportRightBoundary = rect.width;
            Rect columnRect = new(-columnsOffsetX, 0f, 0f, rect.height);
            for (int i = 0; i < columns.Length; i++)
            {
                Column column = columns[i];
                float columnWidth = column.Width;
                columnRect.width = columnWidth;
                float columnRectRightBoundary = columnRect.xMax;

                if (columnRectRightBoundary > 0f)
                {
                    DrawColumn(columnRect, column, i, visibleUnpinnedRows, visibleUnpinnedRowsOffsetY);
                    if (i < columns.Length - 1)
                    {
                        Widgets.Draw.VerticalLine(columnRectRightBoundary - 1f, 0f, rect.height, _columnSeparatorLineColor);
                    }
                }

                if (columnRectRightBoundary >= viewportRightBoundary)
                {
                    break;
                }

                columnRect.x = columnRectRightBoundary;
            }
        }

        // Horizontal scrolling
        // Register mouse-drag only below headers to not interfere with them.
        //Event currentEvent = Event.current;
        //if (doHorScroll && currentEvent.type == EventType.MouseDrag && Mouse.IsOver(rect))
        //{
        //    _scrollPosition.x = Mathf.Max(_scrollPosition.x + currentEvent.delta.x * -1f, 0f);
        //}
    }

    private void DrawColumn(Rect rect, Column column, int columnIndex, ReadOnlyListSegment<int> visibleUnpinnedRows, float visibleUnpinnedRowsOffsetY)
    {
        using (new GUIClipContext(rect))
        {
            Rect innerRect = new(0f, 0f, rect.width, rect.height);

            // Header
            Rect headerRect = innerRect.CutByY(_rowHeight);
            column.DrawHeaderCell(headerRect, columnIndex);

            // Pinned rows
            ReadOnlyListSegment<int> pinnedRows = PinnedRows;
            if (pinnedRows.Length > 0)
            {
                Rect pinnedRowsRect = innerRect.CutByY(_pinnedRowsHeight);
                DrawColumnCells(pinnedRowsRect, column, pinnedRows, 0f);
            }

            // Unpinned rows
            if (visibleUnpinnedRows.Length > 0)
            {
                DrawColumnCells(innerRect, column, visibleUnpinnedRows, visibleUnpinnedRowsOffsetY);
            }
        }
    }

    private void DrawColumnCells(Rect rect, Column column, ReadOnlyListSegment<int> rows, float offsetY)
    {
        using (new GUIClipContext(rect))
        {
            Rect cellRect = new(0f, -offsetY, rect.width, _rowHeight);
            for (int i = 0; i < rows.Length; i++)
            {
                int row = rows[i];
                try
                {
                    column.Worker.DrawCell(cellRect, row);
                }
                catch
                {
                    // TODO?
                }
                cellRect.y += _rowHeight;
            }
        }
    }

    private void DrawBackground(Rect rect, ReadOnlyListSegment<int> visibleUnpinnedRows, float visibleUnpinnedRowsOffsetY)
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
        ReadOnlyListSegment<int> pinnedRows = PinnedRows;
        if (pinnedRows.Length > 0)
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
            using (new GUIClipContext(rect))
            {
                Rect rowRect = new(0f, -visibleUnpinnedRowsOffsetY, rect.width, _rowHeight);
                for (int i = 0; i < visibleUnpinnedRows.Length; i++)
                {
                    //int row = visibleUnpinnedRows[i];
                    if (Mouse.IsOver(rowRect))
                    {
                        Verse.Widgets.DrawHighlight(rowRect);
                    }
                    else if ((visibleUnpinnedRows.Start + i) % 2 == 0)
                    {
                        Verse.Widgets.DrawLightHighlight(rowRect);
                    }
                    rowRect.y += _rowHeight;
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
