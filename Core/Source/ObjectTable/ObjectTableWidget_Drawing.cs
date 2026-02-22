using System.Collections.Generic;
using System.Linq;
using Stats.MainTabWindow;
using Stats.Widgets;
using UnityEngine;
using UnityEngine.UIElements;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    public override void Draw(Rect rect, bool showSettingsMenu)
    {
        if (Event.current.type == EventType.Layout)
        {
            _guiAction?.Invoke();
        }

        //if (showSettingsMenu)
        //{
        //    DrawColumnsTab(ref rect);
        //}

        // Probably could cache these.
        float pinnedColumnsWidth = 0f;
        foreach (var column in _pinnedColumns)
        {
            pinnedColumnsWidth += column.Width;
        }

        float unpinnedColumnsWidth = 0f;
        foreach (var column in _unpinnedColumns)
        {
            unpinnedColumnsWidth += column.Width;
        }

        float contentWidth = pinnedColumnsWidth + unpinnedColumnsWidth;
        float contentHeight = _headerRowHeight + _pinnedRowsHeight + _unpinnedRowsHeight;
        Vector2 contentSize = new(contentWidth, contentHeight);
        float viewportWidth = _unpinnedRowsHeight > 0f
                ? rect.width - GenUI.ScrollBarWidth// Will scroll vertically
                : rect.width;
        float viewportHeight = contentWidth > rect.width
                ? rect.height - GenUI.ScrollBarWidth// Will scroll horizontally
                : rect.height;
        Vector2 viewportSize = new(viewportWidth, viewportHeight);
        Rect contentRect = new(Vector2.zero, Vector2.Max(contentSize, viewportSize));
        // Add empty space for more convenient vertical scrolling.
        contentRect.height += Mathf.Min(contentHeight, viewportHeight) - _headerRowHeight - _pinnedRowsHeight;

        Verse.Widgets.BeginScrollView(rect, ref _scrollPosition, contentRect, true);

        Rect viewportRect = new(_scrollPosition, viewportSize);

        // Left part
        if (pinnedColumnsWidth > 0f)
        {
            Rect leftPartRect = viewportRect.CutByX(pinnedColumnsWidth);

            DrawPart(leftPartRect, _pinnedColumns, _scrollPosition with { x = 0f }, false);

            // Separator line
            Widgets.Draw.VerticalLine(leftPartRect.xMax - 1f, leftPartRect.y, rect.height, MainTabWindowWidget.BorderLineColor);
        }

        // Right part
        if (unpinnedColumnsWidth > 0f)
        {
            DrawPart(viewportRect, _unpinnedColumns, _scrollPosition, true);
        }

        Verse.Widgets.EndScrollView();
    }

    private void DrawPart(Rect rect, List<Column> columns, Vector2 scrollPosition, bool doHorScroll)
    {
        Rect headersRect = rect.CutByY(_headerRowHeight);

        if (Event.current.type == EventType.Repaint)
        {
            DrawColumnSeparators(rect, columns, scrollPosition.x);
        }

        DrawHeaders(headersRect, columns, scrollPosition.x);

        if (_pinnedRows.Count > 0)
        {
            Rect pinnedRowsRect = rect.CutByY(_pinnedRowsHeight);

            DrawPinnedRows(pinnedRowsRect, columns, scrollPosition);
        }

        DrawRows(rect, _unpinnedRows, columns, scrollPosition, true);
    }

    private void DrawHeaders(Rect rect, List<Column> columns, float offsetX)
    {
        GUI.BeginClip(rect);

        // TODO
        Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width, MainTabWindowWidget.BorderLineColor);

        GUI.EndClip();
    }

    private void DrawPinnedRows(Rect rect, List<Column> columns, Vector2 scrollPosition)
    {
        Verse.Widgets.DrawStrongHighlight(rect, _pinnedRowsBGColor);
        DrawRows(rect, _pinnedRows, columns, scrollPosition with { y = 0f }, false);
        Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width, MainTabWindowWidget.BorderLineColor);
    }

    private void DrawRows(Rect rect, List<Row> rows, List<Column> columns, Vector2 scrollPosition, bool doHorScroll)
    {
        // Hor scrolling
        // Register mouse-drag only below headers to not interfere with them.
        if (doHorScroll && Event.current.type == EventType.MouseDrag && Mouse.IsOver(rect))
        {
            _scrollPosition.x = Mathf.Max(_scrollPosition.x + Event.current.delta.x * -1f, 0f);
        }

        GUI.BeginClip(rect);

        float yMax = rect.height;
        rect.x = 0f;
        rect.y = -scrollPosition.y;

        for (int i = 0; i < rows.Count; i++)
        {
            Row row = rows[i];

            if (rect.y >= yMax)
            {
                break;
            }

            rect.height = row.Height;

            if (rect.yMax > 0f)
            {
                row.Draw(rect, columns, scrollPosition.x, i);
            }

            rect.y = rect.yMax;
        }

        GUI.EndClip();
    }

    // The performance impact of instead drawing a vertical border for each
    // individual column's cell is huge. So we have to keep this.
    private void DrawColumnSeparators(Rect rect, List<Column> columns, float offsetX)
    {
        float x = -offsetX;

        for (int i = 0; i < columns.Count; i++)
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
