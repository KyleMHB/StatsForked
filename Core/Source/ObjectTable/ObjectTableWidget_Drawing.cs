using System.Collections.Generic;
using Stats.MainTabWindow;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    public override void Draw(Rect rect, bool showSettingsMenu)
    {
        if (Event.current.type == EventType.Layout)
        {
            if (_rowToPinOrUnpin != null)
            {
                if (_rowToPinOrUnpin.IsPinned)
                {
                    _unpinnedRows.Remove(_rowToPinOrUnpin);
                    _pinnedRows.Add(_rowToPinOrUnpin);
                }
                else
                {
                    _pinnedRows.Remove(_rowToPinOrUnpin);
                    _unpinnedRows.Add(_rowToPinOrUnpin);
                }
            }
        }

        //if (showSettingsMenu)
        //{
        //    DrawColumnsTab(ref rect);
        //}

        // Probably could cache these.
        float leftColumnsMinWidth = 0f;
        foreach (var column in ColumnsVisiblePinned)
        {
            leftColumnsMinWidth += column.Width;
        }

        float rightColumnsMinWidth = 0f;
        foreach (var column in ColumnsVisibleUnpinned)
        {
            rightColumnsMinWidth += column.Width;
        }

        Vector2 contentSizeMax = new(
            // Min. row width
            leftColumnsMinWidth + rightColumnsMinWidth,
            // Total rows height
            _headerRowHeight + _pinnedRowsHeight + _filteredUnpinnedRowsHeight
        );
        Vector2 contentSizeVisible = new(
            // Will scroll vertically
            _filteredUnpinnedRowsHeight > 0f
                ? rect.width - GenUI.ScrollBarWidth
                : rect.width,
            // Will scroll horizontally
            contentSizeMax.x > rect.width
                ? rect.height - GenUI.ScrollBarWidth
                : rect.height
        );
        Rect contentRectMax = new(
            Vector2.zero,
            Vector2.Max(contentSizeMax, contentSizeVisible)
        );
        // Adds empty space for more convenient vertical scrolling.
        contentRectMax.height += Mathf.Min(contentSizeMax.y, contentSizeVisible.y) - _headerRowHeight - _pinnedRowsHeight;

        Verse.Widgets.BeginScrollView(rect, ref _scrollPosition, contentRectMax, true);

        Rect contentRectVisible = new(_scrollPosition, contentSizeVisible);

        // Left part
        if (leftColumnsMinWidth > 0f)
        {
            Rect leftPartRect = contentRectVisible.CutByX(leftColumnsMinWidth);

            DrawPart(leftPartRect, _columns, _scrollPosition with { x = 0f }, false);

            // Separator line
            Widgets.Draw.VerticalLine(
                leftPartRect.xMax - 1f,
                leftPartRect.y,
                rect.height,
                MainTabWindowWidget.BorderLineColor
            );
        }

        // Right part
        if (rightColumnsMinWidth > 0f)
        {
            DrawPart(contentRectVisible, _columns, _scrollPosition, true);
        }

        Verse.Widgets.EndScrollView();
    }

    private void DrawPart(Rect rect, Range columnsRange, Vector2 scrollPosition, bool doHorScroll)
    {
        Rect headersRect = rect.CutByY(_headerRowHeight);

        DrawColumnSeparators(rect, columnsRange, scrollPosition.x);

        DrawHeaders(headersRect, columnsRange, scrollPosition.x);

        if (_pinnedRows.Count > 0)
        {
            Rect pinnedBodyRowsRect = rect.CutByY(_pinnedRowsHeight);

            Verse.Widgets.DrawStrongHighlight(pinnedBodyRowsRect, PinnedRowsBGColor);
            DrawRows(pinnedBodyRowsRect, _pinnedRows, columnsRange, scrollPosition with { y = 0f }, false);
            Verse.Widgets.DrawLineHorizontal(
                pinnedBodyRowsRect.x,
                pinnedBodyRowsRect.yMax - 1f,
                rect.width,
                MainTabWindowWidget.BorderLineColor
            );
        }

        DrawRows(rect, _filteredUnpinnedRows, columnsRange, scrollPosition, true);
    }

    private void DrawHeaders(Rect rect, Range columnsRange, float offsetX)
    {
        GUI.BeginClip(rect);

        // TODO
        Verse.Widgets.DrawLineHorizontal(rect.x, rect.yMax - 1f, rect.width, MainTabWindowWidget.BorderLineColor);

        GUI.EndClip();
    }

    private void DrawRows(Rect rect, List<Row> rows, Range columnsRange, Vector2 scrollPosition, bool doHorScroll)
    {
        // Hor scrolling
        // Register mouse-drag only below headers row to not interfere with them.
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
                row.Draw(rect, _columns, columnsRange, scrollPosition.x, i);
            }

            rect.y = rect.yMax;
        }

        GUI.EndClip();
    }

    // The performance impact of instead drawing a vertical border for each
    // individual column's cell is huge. So we have to keep this.
    private void DrawColumnSeparators(Rect rect, Range columnsRange, float offsetX)
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        float x = -offsetX;

        for (int i = columnsRange.Start; i < columnsRange.End; i++)
        {
            Column column = _columns[i];
            x += column.Width;

            if (x >= rect.width)
            {
                break;
            }

            if (x > 0f)
            {
                Widgets.Draw.VerticalLine(
                    x + rect.x - 1f,
                    rect.y,
                    rect.height,
                    ColumnSeparatorLineColor
                );
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
