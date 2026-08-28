using System;
using System.Runtime.CompilerServices;
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
        using MultiValueDisplay.Scope displayScope = MultiValueDisplay.Enter(_expandMultiValueCells);
        if (_beforeDraw != null)
        {
            _beforeDraw.Invoke();
            _beforeDraw = null;
        }

        if (Event.current.type == EventType.Layout)
        {
            RefreshLiveCells();
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

    private void RefreshLiveCells()
    {
        bool anyColumnChanged = false;
        if (HasActiveLiveTableFilter() && InventoryStateTracker.RefreshIfNeeded())
        {
            anyColumnChanged = true;
        }

        int columnsCount = _columns.Count;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = _columns[i];
            if (column.IsRefreshable && column.RefreshCells())
            {
                anyColumnChanged = true;
            }
        }

        foreach (Column column in _filterColumns.Values)
        {
            if (column.IsRefreshable && column.RefreshCells())
            {
                anyColumnChanged = true;
            }
        }

        if (anyColumnChanged)
        {
            SortRows();
            ApplyFilters();
        }
    }

    private void DrawVisibleContent(Rect rect)
    {
        Event @event = Event.current;
        Vector2 scrollPosition = _scrollPosition;
        float bottomRowsRectHeight = rect.height - HeadersRowHeight - _topRowsHeight;
        GetVisibleBottomRows(
            scrollPosition.y,
            bottomRowsRectHeight,
            out int visibleBottomRowsStart,
            out int visibleBottomRowsCount,
            out float firstVisibleBottomRowY);
        int topRowsCount = _topRowsCount;

        Span<int> visibleBottomRows = stackalloc int[visibleBottomRowsCount];
        _rows.CopyTo(visibleBottomRows, visibleBottomRowsStart);

        Span<int> topRows = stackalloc int[topRowsCount];
        _rows.CopyTo(topRows);

        // Layout
        rect
            .CutLeft(out Rect leftColumnsRect, _leftColumnsWidth)
            .TakeRest(out Rect rightColumnsRect)
            .CutTop(HeadersRowHeight)// Register mouse-drag only below headers to not interfere with them.
            .TakeRest(out Rect mouseDragScrollAreaRect);

        // Rows
        DrawRows(rect, visibleBottomRowsStart, visibleBottomRowsCount, firstVisibleBottomRowY);

        // Pinned columns
        if (_leftColumnsCount > 0)
        {
            DrawColumns(leftColumnsRect, Vector2.zero, LeftColumns, topRows, visibleBottomRows, visibleBottomRowsStart, firstVisibleBottomRowY);
            // Separator line
            if (@event.type == EventType.Repaint)
            {
                leftColumnsRect.DrawBorderRight(FixedPartSeparatorLineColor);
            }
        }

        // Unpinned columns
        if (RightColumnsCount > 0)
        {
            using (new GUIClipScope(rightColumnsRect, new Vector2(-scrollPosition.x, 0f)))
            {
                DrawColumns(rightColumnsRect with { x = 0f, y = 0f }, scrollPosition, RightColumns, topRows, visibleBottomRows, visibleBottomRowsStart, firstVisibleBottomRowY);
            }
        }

        DoHorScrollControl(mouseDragScrollAreaRect);
    }

    private void GetVisibleBottomRows(
        float scrollY,
        float viewportHeight,
        out int start,
        out int count,
        out float firstRowY)
    {
        start = _topRowsCount;
        float heightBeforeStart = 0f;
        while (start < _rows.Count)
        {
            float rowHeight = GetRowHeight(start);
            if (heightBeforeStart + rowHeight > scrollY)
            {
                break;
            }

            heightBeforeStart += rowHeight;
            start++;
        }

        firstRowY = heightBeforeStart - scrollY;
        count = 0;
        float visibleHeight = firstRowY;
        while (start + count < _rows.Count && visibleHeight < viewportHeight)
        {
            visibleHeight += GetRowHeight(start + count);
            count++;
        }
    }

    private void DrawColumns(Rect rect, Vector2 scrollPosition, ReadOnlyListSegment<Column> columns, Span<int> topRows, Span<int> bottomRows, int bottomRowsStart, float bottomRowsY)
    {
        Event @event = Event.current;
        float scrollX = scrollPosition.x;
        float xMin = rect.xMin + scrollX;
        float xMax = rect.xMax + scrollX;
        float mouseX = @event.mousePosition.x;
        bool mouseXIsInVisibleArea = xMin < mouseX && mouseX < xMax;
        ref Rect columnRect = ref rect;
        int columnsCount = columns.Length;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            columnRect.width = column.Width;
            float columnRectXmax = columnRect.xMax;

            if (xMin < columnRectXmax && columnRect.xMin < xMax)
            {
                column.Draw(columnRect, topRows, bottomRows, bottomRowsStart, bottomRowsY, mouseXIsInVisibleArea);
            }
            else if (column.IsResized)
            {
                column.DoResize();
            }

            columnRect.x = columnRectXmax;
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
                .Fill(HeadersRowBGColor)
                .DrawBorderBottom(ColumnSeparatorLineColor);
        }

        // Pinned rows
        int topRowsCount = _topRowsCount;
        if (topRowsCount > 0)
        {
            if (isRepaint)
            {
                topRowsRect.DrawBorderBottom(FixedPartSeparatorLineColor);
            }

            // Rows
            Rect rowRect = topRowsRect;
            for (int i = 0; i < topRowsCount; i++)
            {
                rowRect.height = GetRowHeight(i);
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
            float firstRowHeight = GetRowHeight(bottomRowsStart) + bottomRowsY;
            int bottomRowsEnd = bottomRowsCount + bottomRowsStart;// Exclusive
            Rect rowRect = bottomRowsRect with { height = firstRowHeight };
            for (int i = bottomRowsStart; i < bottomRowsEnd; i++)
            {
                DrawRow(rowRect, i);

                rowRect.y = rowRect.yMax;
                if (i + 1 < bottomRowsEnd)
                {
                    rowRect.height = GetRowHeight(i + 1);
                }

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
        Event @event = Event.current;

        if (@event.type == EventType.Repaint)
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

        if (mouseIsOverRect && @event is { type: EventType.MouseUp, button: 0, modifiers: EventModifiers.Control })
        {
            HandleRowPin(index);
            GUIUtils.ReleaseMouseControl();
            @event.Use();
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

    private void DoHorScrollControl(Rect rect)
    {
        Event @event = Event.current;

        if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && Mouse.IsOver(rect))
        {
            _rightPartIsPanned = true;
        }
        else if (_rightPartIsPanned)
        {
            if (OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                _scrollPosition.x = Mathf.Max(_scrollPosition.x - @event.delta.x, 0f);
                @event.Use();
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                _rightPartIsPanned = false;
                GUIUtils.ReleaseMouseControl();
                @event.Use();
            }
        }

        // This button is here to capture control from whatever
        // eats the events above horizontal scroll code.
        GUI.Button(rect, GUIContent.none, GUIStyle.none);
    }
}
