using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    public override void Draw(Rect rect, bool showSettingsMenu)
    {
        if (showSettingsMenu)
        {
            DrawColumnsTab(ref rect);
        }

        if (Event.current.type == EventType.Layout)
        {
            DoTasks();

            // Skip the event? This coould save us around 2ms/frame, but is it safe to do?
            // Maybe skip only if some tasks are scheduled.
        }

        // Probably could cache these.
        var leftColumnsMinWidth = 0f;
        foreach (var column in ColumnsVisiblePinned)
        {
            leftColumnsMinWidth += column.Width;
        }

        var rightColumnsMinWidth = 0f;
        foreach (var column in ColumnsVisibleUnpinned)
        {
            rightColumnsMinWidth += column.Width;
        }

        var contentSizeMax = new Vector2(
            // Min. row width
            leftColumnsMinWidth + rightColumnsMinWidth,
            // Total rows height
            HeaderRowsHeight + PinnedRowsHeight + UnpinnedRowsHeight
        );
        var contentSizeVisible = new Vector2(
            // Will scroll vertically
            UnpinnedRowsHeight > 0f
                ? rect.width - GenUI.ScrollBarWidth
                : rect.width,
            // Will scroll horizontally
            contentSizeMax.x > rect.width
                ? rect.height - GenUI.ScrollBarWidth
                : rect.height
        );
        var contentRectMax = new Rect(
            Vector2.zero,
            Vector2.Max(contentSizeMax, contentSizeVisible)
        );
        // Adds empty space for more convenient vertical scrolling.
        contentRectMax.height += Mathf.Min(contentSizeMax.y, contentSizeVisible.y) - HeaderRowsHeight - PinnedRowsHeight;

        Verse.Widgets.BeginScrollView(rect, ref ScrollPosition, contentRectMax, true);

        var contentRectVisible = new Rect(ScrollPosition, contentSizeVisible);

        // Left part
        if (leftColumnsMinWidth > 0f)
        {
            var leftPartRect = contentRectVisible.CutByX(leftColumnsMinWidth);

            DrawPart(leftPartRect, ColumnsVisiblePinned, ScrollPosition with { x = 0f }, 0f);

            // Separator line
            Widgets.Draw.VerticalLine(
                leftPartRect.xMax - 1f,
                leftPartRect.y,
                rect.height,
                MainTabWindow.BorderLineColor
            );
        }

        // Right part
        var rightPartFreeSpace = contentRectVisible.width - rightColumnsMinWidth;
        var cellExtraWidth = Mathf.Max(rightPartFreeSpace / ColumnsVisibleUnpinned.Count, 0f);

        DrawPart(contentRectVisible, ColumnsVisibleUnpinned, ScrollPosition, cellExtraWidth);

        Verse.Widgets.EndScrollView();
    }
    private void DrawPart(
        Rect rect,
        List<ColumnWorker> columns,
        Vector2 scrollPosition,
        float cellExtraWidth
    )
    {
        if (columns.Count == 0)
            return;

        var origRect = rect;
        var headersRect = rect.CutByY(HeaderRowsHeight);
        var horScrollPosition = scrollPosition with { y = 0f };

        DrawRows(headersRect, HeaderRows, columns, horScrollPosition, cellExtraWidth);
        Verse.Widgets.DrawLineHorizontal(
            headersRect.x,
            headersRect.yMax - 1f,
            rect.width,
            MainTabWindow.BorderLineColor
        );

        if (columns == ColumnsVisibleUnpinned)
        {
            // Register mouse-drag only below headers row to not interfere with filter inputs.
            DoHorScroll(rect, ref ScrollPosition);
        }

        if (PinnedRows.Count > 0)
        {
            var pinnedBodyRowsRect = rect.CutByY(PinnedRowsHeight);

            Verse.Widgets.DrawStrongHighlight(pinnedBodyRowsRect, PinnedRowsBGColor);
            DrawRows(pinnedBodyRowsRect, PinnedRows, columns, horScrollPosition, cellExtraWidth);
            Verse.Widgets.DrawLineHorizontal(
                pinnedBodyRowsRect.x,
                pinnedBodyRowsRect.yMax - 1f,
                rect.width,
                MainTabWindow.BorderLineColor
            );
        }

        DrawRows(rect, UnpinnedRows, columns, scrollPosition, cellExtraWidth);

        DrawColumnSeparators(origRect, columns, scrollPosition.x, cellExtraWidth);
    }
    private void DrawRows(
        Rect rect,
        IReadOnlyCollection<Row> rows,
        List<ColumnWorker> columns,
        Vector2 scrollPosition,
        float cellExtraWidth
    )
    {
        GUI.BeginClip(rect);

        var yMax = rect.height;
        rect.x = 0f;
        rect.y = -scrollPosition.y;
        var i = 0;

        foreach (var row in rows)
        {
            if (row.IsVisible == false)
                continue;

            if (rect.y >= yMax)
                break;

            rect.height = row.Height;

            if (rect.yMax > 0f)
            {
                var rowWasClicked = row.Draw(rect, columns, scrollPosition.x, cellExtraWidth, i);

                if (rowWasClicked && row is ObjectRow objectRow)
                {
                    if (PinnedRows.Contains(objectRow))
                    {
                        UnpinRow(objectRow);
                    }
                    else
                    {
                        PinRow(objectRow);
                    }

                    return;
                }
            }

            rect.y = rect.yMax;
            i++;
        }

        GUI.EndClip();
    }
    // The performance impact of instead drawing a vertical border for each
    // individual column's cell is huge. So we have to keep this.
    private static void DrawColumnSeparators(
        Rect rect,
        List<ColumnWorker> columns,
        float offsetX,
        float cellExtraWidth
    )
    {
        if (Event.current.type != EventType.Repaint)
            return;

        var x = -offsetX;

        foreach (var column in columns)
        {
            x += column.Width + cellExtraWidth;

            if (x >= rect.width)
                break;

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
    private void DrawColumnsTab(ref Rect rect)
    {
        var columnsTabWidgetSize = ColumnsTabWidget.GetSize(rect.size);
        var columnsTabRect = rect.CutByX(columnsTabWidgetSize.x + GenUI.ScrollBarWidth);
        var columnsTabRectMax = new Rect(Vector2.zero, columnsTabWidgetSize);
        // Adds empty space for more convenient vertical scrolling.
        columnsTabRectMax.height += columnsTabRect.height;

        Verse.Widgets.BeginScrollView(columnsTabRect, ref ColumnsTabScrollPosition, columnsTabRectMax, true);
        ColumnsTabWidget.DrawIn(columnsTabRectMax);
        Verse.Widgets.EndScrollView();
        Widgets.Draw.VerticalLine(
            columnsTabRect.xMax,
            rect.y,
            rect.height,
            MainTabWindow.BorderLineColor
        );
        rect.xMin += 1f;
    }
    private static void DoHorScroll(Rect rect, ref Vector2 scrollPosition)
    {
        if (Event.current.type == EventType.MouseDrag && Mouse.IsOver(rect))
        {
            scrollPosition.x = Mathf.Max(scrollPosition.x + Event.current.delta.x * -1f, 0f);

            // Why no "Event.current.Use();"? Because the thing locks itself on mouse-up.
        }
    }
    private void DoTasks()
    {
        if (DoUpdateCachedColumns)
        {
            UpdateCachedColumns();
        }

        //if (DoRefreshColumns)
        if (ColumnsToRefresh.Count > 0)
        {
            ColumnsToRefresh.Pop().RefreshCells();
            //RefreshColumns();
        }
        else if (DoFilter)
        {
            ApplyFilters();
        }
        else if (DoSort)
        {
            SortRows();
        }
        else if (DoResize)
        {
            Resize();
        }
        else
        {
            //DoRefreshColumns = true;

            // TODO: Do not add "static" columns.
            foreach (var column in Columns)
            {
                ColumnsToRefresh.Push(column);
            }
        }
    }
    private void Resize()
    {
        HeaderRowsHeight = 0f;
        foreach (var row in HeaderRows)
        {
            HeaderRowsHeight += row.Resize(ColumnsVisible);
        }

        PinnedRowsHeight = 0f;
        foreach (var row in PinnedRows)
        {
            PinnedRowsHeight += row.Resize(ColumnsVisible);
        }

        UnpinnedRowsHeight = 0f;
        foreach (var row in UnpinnedRows)
        {
            if (row.IsVisible)
            {
                UnpinnedRowsHeight += row.Resize(ColumnsVisible);
            }
        }

        DoResize = false;
    }
}
