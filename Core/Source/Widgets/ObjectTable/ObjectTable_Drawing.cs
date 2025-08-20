using System;
using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    public override void Draw(Rect rect, bool showSettingsMenu)
    {
        // Columns tab
        if (showSettingsMenu)
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

        if (Event.current.type == EventType.Layout)
        {
            if (DoFilter)
            {
                foreach (var row in UnpinnedRows)
                {
                    // Evaluate to "true", so the error would be noticeable.
                    var rowIsValid = true;

                    try
                    {
                        rowIsValid = ObjectMatchesFilters(row.Object, ActiveFilters);
                    }
                    catch (Exception e)
                    {
                        Log.Error(e.Message);
                    }

                    row.IsVisible = rowIsValid;
                }

                DoFilter = false;
                DoResize = true;
            }

            if (DoSort)
            {
                SortRows(PinnedRows);
                SortRows(UnpinnedRows);

                DoSort = false;
            }

            if (DoResize)
            {
                Resize();

                DoResize = false;
            }
        }

        // Probably could cache this.
        var leftColumnsMinWidth = 0f;
        var rightColumnsMinWidth = 0f;
        var rightColumnsCount = 0;

        foreach (var column in Columns)
        {
            if (column.IsVisible == false)
            {
                continue;
            }

            if (column.IsPinned)
            {
                leftColumnsMinWidth += column.Width;
            }
            else
            {
                rightColumnsMinWidth += column.Width;
                rightColumnsCount++;
            }
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

            DrawPart(leftPartRect, ScrollPosition with { x = 0f }, 0f, true);

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
        var cellExtraWidth = Mathf.Max(rightPartFreeSpace / rightColumnsCount, 0f);

        DrawPart(contentRectVisible, ScrollPosition, cellExtraWidth, false);

        Verse.Widgets.EndScrollView();
    }
    private void DrawPart(
        Rect rect,
        Vector2 scrollPosition,
        float cellExtraWidth,
        bool drawPinnedColumns
    )
    {
        var origRect = rect;
        var headersRect = rect.CutByY(HeaderRowsHeight);
        var horScrollPosition = scrollPosition with { y = 0f };

        DrawRows(headersRect, HeaderRows, horScrollPosition, cellExtraWidth, drawPinnedColumns);
        Verse.Widgets.DrawLineHorizontal(
            headersRect.x,
            headersRect.yMax - 1f,
            rect.width,
            MainTabWindow.BorderLineColor
        );

        if (drawPinnedColumns == false)
        {
            // Register mouse-drag only below headers row to not interfere with filter inputs.
            DoHorScroll(rect, ref ScrollPosition);
        }

        if (PinnedRows.Count > 0)
        {
            var pinnedBodyRowsRect = rect.CutByY(PinnedRowsHeight);

            Verse.Widgets.DrawStrongHighlight(pinnedBodyRowsRect, PinnedRowsBGColor);
            DrawRows(pinnedBodyRowsRect, PinnedRows, horScrollPosition, cellExtraWidth, drawPinnedColumns);
            Verse.Widgets.DrawLineHorizontal(
                pinnedBodyRowsRect.x,
                pinnedBodyRowsRect.yMax - 1f,
                rect.width,
                MainTabWindow.BorderLineColor
            );
        }

        DrawRows(rect, UnpinnedRows, scrollPosition, cellExtraWidth, drawPinnedColumns);

        DrawColumnSeparators(origRect, Columns, scrollPosition.x, cellExtraWidth, drawPinnedColumns);
    }
    private void DrawRows(
        Rect rect,
        IReadOnlyCollection<Row> rows,
        Vector2 scrollPosition,
        float cellExtraWidth,
        bool drawPinnedColumns
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
            {
                continue;
            }

            if (rect.y >= yMax)
            {
                break;
            }

            rect.height = row.Height;
            if (rect.yMax > 0f)
            {
                var rowWasClicked = row.Draw(rect, scrollPosition.x, drawPinnedColumns, cellExtraWidth, i);

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
        List<ColumnWorker<TObject>> columns,
        float offsetX,
        float cellExtraWidth,
        bool drawPinnedColumns
    )
    {
        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        var x = -offsetX;

        foreach (var column in columns)
        {
            if (column.IsPinned != drawPinnedColumns || column.IsVisible == false)
            {
                continue;
            }

            x += column.Width + cellExtraWidth;
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
    private static void DoHorScroll(Rect rect, ref Vector2 scrollPosition)
    {
        if (Event.current.type == EventType.MouseDrag && Mouse.IsOver(rect))
        {
            scrollPosition.x = Mathf.Max(scrollPosition.x + Event.current.delta.x * -1f, 0f);

            // Why no "Event.current.Use();"? Because the thing locks itself on mouse-up.
        }
    }
    private void Resize()
    {
        // Resize
        HeaderRowsHeight = 0f;
        PinnedRowsHeight = 0f;
        UnpinnedRowsHeight = 0f;

        foreach (var column in Columns)
        {
            column.Width = 0f;
        }

        foreach (var row in HeaderRows)
        {
            row.Resize();
            HeaderRowsHeight += row.Height;
        }

        foreach (var row in PinnedRows)
        {
            row.Resize();
            PinnedRowsHeight += row.Height;
        }

        foreach (var row in UnpinnedRows)
        {
            if (row.IsVisible)
            {
                row.Resize();
                UnpinnedRowsHeight += row.Height;
            }
        }
    }
}
