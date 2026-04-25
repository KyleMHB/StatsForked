using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.GUIScopes;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;
using Verse.Sound;
using static Stats.GUIStyles.Table;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private void PinColumn(int index)
    {
        if (index != _leftColumnsCount)
        {
            int lastPinnedColumnIndex = _leftColumnsCount - 1;
            _columns.MoveAfterElemAt(index, lastPinnedColumnIndex);
        }
        _leftColumnsCount++;
    }

    private void UnpinColumn(int index)
    {
        int lastPinnedColumnIndex = _leftColumnsCount - 1;
        if (index != lastPinnedColumnIndex)
        {
            _columns.MoveAfterElemAt(index, lastPinnedColumnIndex);
        }
        _leftColumnsCount--;
    }

    private void AddColumn(ColumnDef columnDef)
    {
        TryAddColumn(columnDef, notifyToolbar: true, applyFilters: true);
    }

    private bool TryAddColumn(ColumnDef columnDef, bool notifyToolbar, bool applyFilters)
    {
        Type workerClass = columnDef.workerClass;
        if (typeof(ColumnWorker<TObject>).IsAssignableFrom(workerClass) == false)
        {
            WarnIncompatibleColumn(columnDef.defName, _tableWorker.Def.defName);
            return false;
        }

        ColumnWorker<TObject> columnWorker = (ColumnWorker<TObject>)Activator.CreateInstance(workerClass, columnDef);
        ICollection<CellField> cellFields = columnWorker.GetCellFields(_tableWorker);
        Column column = new(columnWorker, _tableWorker, this, cellFields);
        _columns.Add(column);
        columnWorker.NotifyRowAdded(_objects);
        RegisterColumnFilters(column, cellFields);
        if (_sortColumn == null)
        {
            _sortColumn = column;
            SortRows();
        }
        if (applyFilters)
        {
            ApplyFilters();
        }
        if (notifyToolbar)
        {
            _toolbar.NotifyColumnAdded(column);
        }

        return true;
    }

    private void RemoveColumn(int index)
    {
        if (index < _leftColumnsCount)
        {
            _leftColumnsCount--;
        }

        Column column = _columns[index];
        _toolbar.NotifyColumnRemoved(column);
        UnregisterColumnFilters(column);
        _columns.RemoveAt(index);
        if (_pressedColumn == column)
        {
            _pressedColumn = null;
        }
        if (_reorderedColumn == column)
        {
            _reorderedColumn = null;
        }
        if (_sortColumn == column)
        {
            _sortColumn = _columns.Count > 0 ? _columns[0] : null;
        }
        SortRows();
        ApplyFilters();
    }

    private void RemoveColumn(Column column)
    {
        int index = _columns.IndexOf(column);
        RemoveColumn(index);
    }

    private void RemoveColumn(ColumnDef columnDef)
    {
        int index = _columns.FindIndex(column => column.Def == columnDef);
        RemoveColumn(index);
    }

    private sealed class Column
    {
        public float Width { get; private set; }
        public ColumnDef Def { get; }
        public bool IsRefreshable => _worker.IsRefreshable;
        public bool IsManuallyResized { get; private set; }
        public bool IsResized { get; private set; }
        public Comparison<int>? SortComparison { get; }

        private readonly ColumnWorker<TObject> _worker;
        private readonly Widget _titleWidget;
        private readonly TipSignal _tooltip;
        private readonly ObjectTable<TObject> _parent;
        private readonly FloatMenu _menu;

        public Column(ColumnWorker<TObject> worker, TableWorker tableWorker, ObjectTable<TObject> parent, ICollection<CellField> cellFields)
        {
            ColumnDef def = worker.Def;
            Widget titleWidget = def.TitleWidget;

            _worker = worker;
            Def = worker.Def;
            _titleWidget = titleWidget;
            _tooltip = $"<i>{def.LabelCap}</i>\n\n{def.description}";
            _parent = parent;
            SortComparison = cellFields.FirstOrDefault().Compare;
            _menu = new FloatMenu([
                new FloatMenuOption("Sort Asc", () => {
                    parent._sortColumn = this;
                    parent._sortDirection = SortDirectionAscending;
                    parent.SortRows();
                    parent.ApplyFilters();
                }, TexButton.ReorderUp, Color.white),
                new FloatMenuOption("Sort Desc", () => {
                    parent._sortColumn = this;
                    parent._sortDirection = SortDirectionDescending;
                    parent.SortRows();
                    parent.ApplyFilters();
                }, TexButton.ReorderDown, Color.white),
                new FloatMenuOption("Reset width", () => IsManuallyResized = false),
                new FloatMenuOption("Remove", () => parent.RemoveColumn(this), TexButton.Delete, Color.white)
            ]);
        }

        public void Draw(Rect rect, Span<int> topRows, Span<int> bottomRows, float bottomRowsY, bool mouseXIsInVisibleArea)
        {
            bool shouldDrawCellsNow = _worker.ShouldDrawCellsNow;
            rect.CutTop(out Rect headerCellRect, HeadersRowHeight)
                .CutTop(out Rect topRowsRect, _parent._topRowsHeight)
                .TakeRest(out Rect bottomRowsRect);

            DrawHeaderCell(headerCellRect, mouseXIsInVisibleArea);

            if (shouldDrawCellsNow)
            {
                if (topRows.Length > 0)
                {
                    using (new GUIClipScope(topRowsRect))
                    {
                        DrawCells(topRowsRect with { x = 0f, y = 0f }, topRows);
                    }
                }

                if (bottomRows.Length > 0)
                {
                    using (new GUIClipScope(bottomRowsRect, new Vector2(0f, bottomRowsY)))
                    {
                        DrawCells(bottomRowsRect with { x = 0f, y = 0f }, bottomRows);
                    }
                }
            }
        }

        private void DrawHeaderCell(Rect rect, bool mouseXIsInVisibleArea)
        {
            Event @event = Event.current;
            ObjectTable<TObject> parent = _parent;
            ColumnType columnType = _worker.Type;
            const float SideControlMargin = 1f;
            rect.CutLeft(out Rect sortControlRect, GUIStyles.TableCell.PadHor - SideControlMargin)
                .CutRight(out Rect resizeControlRect, GUIStyles.TableCell.PadHor - SideControlMargin)
                .TakeRest(out Rect mainControlRect);

            if (@event.type == EventType.Repaint)
            {
                Rect titleClipRect = mainControlRect.ContractedBy(SideControlMargin, GUIStyles.TableCell.PadVer);
                GUI.BeginClip(titleClipRect);

                float titleWidgetWidth = _titleWidget.Size.x;
                Rect titleRect = titleClipRect with { x = 0f, y = 0f };
                if (columnType == ColumnType.Number)
                {
                    titleRect.CutRight(out titleRect, titleWidgetWidth);
                }
                else if (columnType == ColumnType.Boolean)
                {
                    titleRect.CutMidX(out titleRect, titleWidgetWidth);
                }
                else
                {
                    titleRect = titleRect with { width = titleWidgetWidth };
                }
                _titleWidget.Draw(titleRect);

                GUI.EndClip();

                if (parent._reorderedColumn == this)
                {
                    rect.HighlightSelected();
                }
                else if (Mouse.IsOver(rect))
                {
                    rect.HighlightLight();
                }

                rect.DrawBorderRight(ColumnSeparatorLineColor);
            }

            MouseoverSounds.DoRegion(rect);

            DoSortControl(sortControlRect);
            DoMainControl(mainControlRect, rect, mouseXIsInVisibleArea);
            DoResizeControl(resizeControlRect, mouseXIsInVisibleArea);

            mainControlRect.Tip(_tooltip);
        }

        private void DrawCells(Rect rect, Span<int> rows)
        {
            ColumnWorker<TObject> worker = _worker;
            ref Rect cellRect = ref rect;
            cellRect.height = RowHeight;
            int rowsCount = rows.Length;
            for (int i = 0; i < rowsCount; i++)
            {
                try
                {
                    worker.DrawCell(cellRect, rows[i]);
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

        private void DoMainControl(Rect rect, Rect cellRect, bool mouseXIsInVisibleArea)
        {
            Event @event = Event.current;
            ObjectTable<TObject> parent = _parent;
            bool mouseIsOverRect = Mouse.IsOver(rect);

            if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && mouseIsOverRect)
            {
                parent._pressedColumn = this;
            }
            else if (parent._pressedColumn == this && OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                parent._reorderedColumn = this;
            }

            if (parent._reorderedColumn != null)
            {
                DoReorder(cellRect, parent._reorderedColumn, mouseXIsInVisibleArea);
            }
            else if (@event.type == EventType.MouseUp && mouseIsOverRect)
            {
                if (@event is { button: 0, modifiers: EventModifiers.Control })
                {
                    HandlePin();
                    parent._pressedColumn = null;
                    GUIUtils.ReleaseMouseControl();
                    @event.Use();
                }
                else if (@event is { button: 0, modifiers: EventModifiers.None } && parent._pressedColumn == this)
                {
                    parent.HandleSortRequested(this);
                    parent._pressedColumn = null;
                    GUIUtils.ReleaseMouseControl();
                    @event.Use();
                }
                else if (@event is { button: 1, modifiers: EventModifiers.None })
                {
                    _menu.Open();
                    parent._pressedColumn = null;
                    GUIUtils.ReleaseMouseControl();
                    @event.Use();
                }
            }
            else if (@event.rawType == EventType.MouseUp && parent._pressedColumn == this)
            {
                parent._pressedColumn = null;
            }

            GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        private void DoSortControl(Rect rect)
        {
            ObjectTable<TObject> parent = _parent;
            Event @event = Event.current;
            const float IconPadding = 3f;

            if (@event.type == EventType.Repaint)
            {
                if (parent._sortColumn == this)
                {
                    if (parent._sortDirection == SortDirectionAscending)
                    {
                        rect.TopHalf()
                            .ContractedBy(IconPadding)
                            .DrawTextureFitted(TexButton.ReorderUp);
                    }
                    else
                    {
                        rect.BottomHalf()
                            .ContractedBy(IconPadding)
                            .DrawTextureFitted(TexButton.ReorderDown);
                    }
                }

                if (Mouse.IsOver(rect))
                {
                    rect.Highlight();
                }
            }

            bool wasClicked = GUI.Button(rect, GUIContent.none, GUIStyle.none);
            if (wasClicked && @event is { button: 0, modifiers: EventModifiers.None })
            {
                parent.HandleSortRequested(this);
                parent._pressedColumn = null;
                GUIUtils.ReleaseMouseControl();
                @event.Use();
            }
        }

        private void DoResizeControl(Rect rect, bool mouseXIsInVisibleArea)
        {
            Event @event = Event.current;
            bool mouseIsOverRect = Mouse.IsOver(rect);

            if (@event is { type: EventType.MouseDown, button: 0, modifiers: EventModifiers.None } && mouseIsOverRect)
            {
                IsResized = true;
                IsManuallyResized = true;
            }
            else if (IsResized)
            {
                DoResize();
                // TODO: This will work for both left and right.
                if (mouseXIsInVisibleArea == false)
                {
                    _parent._scrollPosition.x++;
                    Width++;
                }
            }

            if (@event.type == EventType.Repaint && (mouseIsOverRect || IsResized))
            {
                rect.HighlightSelected();
            }

            GUI.Button(rect, GUIContent.none, GUIStyle.none);
        }

        public void DoResize()
        {
            Event @event = Event.current;

            if (OriginalEventUtility.EventType == EventType.MouseDrag)
            {
                Width = Mathf.Clamp(Width + @event.delta.x, HeadersRowHeight, float.MaxValue);
                @event.Use();
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                IsResized = false;
                GUIUtils.ReleaseMouseControl();
                @event.Use();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void DoReorder(Rect rect, Column reorderedColumn, bool mouseXIsInVisibleArea)
        {
            Event @event = Event.current;
            ObjectTable<TObject> parent = _parent;

            if (OriginalEventUtility.EventType == EventType.MouseDrag
                && parent._reorderedColumn != this
                && mouseXIsInVisibleArea
                && rect.x < @event.mousePosition.x && @event.mousePosition.x < rect.xMax)
            {
                List<Column> columns = parent._columns;
                int leftColumnsCount = parent._leftColumnsCount;
                int reorderedColumnIndex = columns.IndexOf(reorderedColumn);
                int thisColumnIndex = columns.IndexOf(this);

                bool reorderedColumnIsPinned = reorderedColumnIndex < leftColumnsCount;
                bool thisColumnIsPinned = thisColumnIndex < leftColumnsCount;
                if (reorderedColumnIsPinned && thisColumnIsPinned == false)
                {
                    parent._leftColumnsCount--;
                }
                else if (reorderedColumnIsPinned == false && thisColumnIsPinned)
                {
                    parent._leftColumnsCount++;
                }

                float xMiddle = rect.x + rect.width / 2f;
                float mouseX = @event.mousePosition.x;
                if (rect.x < mouseX && mouseX < xMiddle)// Left
                {
                    if (thisColumnIndex - 1 != reorderedColumnIndex)
                    {
                        parent._beforeDraw = () => columns.MoveBeforeElemAt(reorderedColumnIndex, thisColumnIndex);
                    }
                }
                // xMiddle < mouseX && mouseX < rect.xMax check is not necessary here
                // because we already checked if mouseX is between rect.x and rect.xMax.
                // So if mouseX is not on the left, it is guaranteed to be on the right.
                else// Right
                {
                    if (thisColumnIndex + 1 != reorderedColumnIndex)
                    {
                        parent._beforeDraw = () => columns.MoveAfterElemAt(reorderedColumnIndex, thisColumnIndex);
                    }
                }

                @event.Use();
            }
            else if (@event.rawType == EventType.MouseUp)
            {
                parent._reorderedColumn = null;
                parent._pressedColumn = null;
                GUIUtils.ReleaseMouseControl();
                @event.Use();
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePin()
        {
            ObjectTable<TObject> parent = _parent;
            int index = parent._columns.IndexOf(this);
            if (index > parent._leftColumnsCount - 1)
            {
                parent._beforeDraw = () => parent.PinColumn(index);
            }
            else
            {
                parent._beforeDraw = () => parent.UnpinColumn(index);
            }
        }

        public void RecalcWidth(List<int> rows)
        {
            Width = Mathf.Max(_titleWidget.Size.x, _worker.GetWidth(rows)) + GUIStyles.TableCell.PadHor * 2f;
        }

        public int CompareRows(int row1, int row2)
        {
            return SortComparison?.Invoke(row1, row2) ?? row1.CompareTo(row2);
        }

        public bool RefreshCells()
        {
            return _worker.RefreshCells();
        }

        public void NotifyParentWindowClosed()
        {
            if (IsResized)
            {
                IsResized = false;
            }
        }
    }
}
