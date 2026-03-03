using System.Collections.Generic;
using System.Linq;
using Stats.MainTabWindow;
using Stats.ObjectTable.Cells;
using Stats.Widgets;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    public override void Draw(Rect rect, bool showSettingsMenu)
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

        Verse.Widgets.BeginScrollView(rect, ref _scrollPosition, contentRect, true);

        Vector2 scrollPosition = _scrollPosition;
        Rect viewportRect = new(scrollPosition, viewportSize);

        // Left part
        int pinnedColumnsCount = _pinnedColumnsCount;
        if (pinnedColumnsCount > 0)
        {
            ReadOnlyListSegment<Column> pinnedColumns = _PinndeColumns;
            float pinnedColumnsWidth = _pinnedColumnsWidth;
            Rect leftPartRect = viewportRect.CutByX(pinnedColumnsWidth);
            DrawPart(leftPartRect, pinnedColumns, scrollPosition with { x = 0f }, false);
            // Separator line
            Widgets.Draw.VerticalLine(leftPartRect.xMax - 1f, leftPartRect.y, rect.height, MainTabWindowWidget.BorderLineColor);
        }

        // Right part
        ReadOnlyListSegment<Column> unpinnedColumns = _UnpinndeColumns;
        if (unpinnedColumns.Length > 0)
        {
            DrawPart(viewportRect, unpinnedColumns, scrollPosition, true);
        }

        Verse.Widgets.EndScrollView();
    }

    private void DrawPart(Rect rect, ReadOnlyListSegment<Column> columns, Vector2 scrollPosition, bool doHorScroll)
    {
        if (Event.current.type == EventType.Repaint)
        {
            DrawColumnSeparators(rect, columns, scrollPosition.x);
        }
        float headerRowHeight = _headerRowHeight;
        Rect headersRect = rect.CutByY(headerRowHeight);
        columns = DrawHeaders(headersRect, columns, scrollPosition.x);

        // Hor scrolling
        // Register mouse-drag only below headers to not interfere with them.
        Event currentEvent = Event.current;
        if (doHorScroll && currentEvent.type == EventType.MouseDrag && Mouse.IsOver(rect))
        {
            _scrollPosition.x = Mathf.Max(_scrollPosition.x + currentEvent.delta.x * -1f, 0f);
        }

        List<Row<TObject>> rows = _filteredRows;
        int rowsCount = rows.Count;
        if (rowsCount == 0)
        {
            return;
        }

        int pinnedRowsCount = _pinnedRowsCount;
        if (pinnedRowsCount > 0)
        {
            ReadOnlyListSegment<Row<TObject>> pinnedRows = _PinndeRows;
            float pinnedRowsHeight = _pinnedRowsHeight;
            Rect pinnedRowsRect = rect.CutByY(pinnedRowsHeight);
            DrawPinnedRows(pinnedRowsRect, pinnedRows, columns, scrollPosition);
        }

        ReadOnlyListSegment<Row<TObject>> unpinnedRows = _UnpinndeRows;
        if (unpinnedRows.Length > 0)
        {
            DrawRows(rect, unpinnedRows, columns, scrollPosition);
        }
    }

    private ReadOnlyListSegment<Column> DrawHeaders(Rect rect, ReadOnlyListSegment<Column> columns, float offsetX)
    {
        GUI.BeginClip(rect);

        Verse.Widgets.DrawHighlight(rect);
        Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width, MainTabWindowWidget.BorderLineColor);

        float viewportRightBoundary = rect.width;
        rect.x = -offsetX;

        int columnsCount = columns.Length;
        int skippedColumnsCount = 0;
        int drawnColumnsCount = 0;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            rect.width = column.Width;
            float cellRightBoundary = rect.xMax;

            if (cellRightBoundary > 0f)
            {
                column.DrawHeaderCell(rect, i);
                drawnColumnsCount++;
            }
            else
            {
                skippedColumnsCount++;
            }

            if (cellRightBoundary >= viewportRightBoundary)
            {
                break;
            }

            rect.x = cellRightBoundary;
        }

        GUI.EndClip();

        return columns.Slice(skippedColumnsCount, drawnColumnsCount);
    }

    private void DrawPinnedRows(Rect rect, ReadOnlyListSegment<Row<TObject>> rows, ReadOnlyListSegment<Column> columns, Vector2 scrollPosition)
    {
        Verse.Widgets.DrawStrongHighlight(rect, _pinnedRowsBGColor);
        Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width, MainTabWindowWidget.BorderLineColor);
        DrawRows(rect, rows, columns, scrollPosition);
    }

    private void DrawRows(Rect rect, ReadOnlyListSegment<Row<TObject>> rows, ReadOnlyListSegment<Column> columns, Vector2 scrollPosition)
    {
        GUI.BeginClip(rect);

        float viewportBottomBoundary = rect.height;
        rect.x = 0f;
        rect.y = -scrollPosition.y;

        int rowsCount = rows.Length;
        for (int i = 0; i < rowsCount; i++)
        {
            Row<TObject> row = rows[i];
            rect.height = row.Height;
            float rowBottomBoundary = rect.yMax;

            if (rowBottomBoundary > 0f)
            {
                row.Draw(rect, columns, scrollPosition.x, i);
            }

            if (rowBottomBoundary >= viewportBottomBoundary)
            {
                break;
            }

            rect.y = rowBottomBoundary;
        }

        GUI.EndClip();
    }

    private void DrawColumnSeparators(Rect rect, ReadOnlyListSegment<Column> columns, float x)
    {
        x *= -1f;

        int columnsCount = columns.Length;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            x += column.Width;

            if (x >= rect.width)
            {
                break;
            }

            if (x > 0f)
            {
                Widgets.Draw.VerticalLine(x + rect.x - 1f, rect.y, rect.height, _columnSeparatorLineColor);
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
