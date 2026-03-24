using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;
using static Stats.GUIStyles.Table;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private void PinColumn(int index)
    {
        List<Column> columns = _columns;
        Column column = columns[index];
        columns.RemoveAt(index);
        columns.Insert(_leftColumnsCount, column);
        _leftColumnsCount++;
    }

    private void UnpinColumn(int index)
    {
        List<Column> columns = _columns;
        Column column = columns[index];
        columns.RemoveAt(index);
        _leftColumnsCount--;
        columns.Insert(_leftColumnsCount, column);
    }

    private void AddColumn(ColumnDef columnDef)
    {
        Type workerClass = columnDef.workerClass;
        ColumnWorker<TObject> columnWorker = (ColumnWorker<TObject>)Activator.CreateInstance(workerClass, columnDef);
        Column column = new(columnWorker, _tableWorker, this);
        _columns.Add(column);
        columnWorker.NotifyRowAdded(_objects);
        _toolbar.NotifyColumnAdded(column);
    }

    private void RemoveColumn(int index)
    {
        if (index < _leftColumnsCount)
        {
            _leftColumnsCount--;
        }

        _toolbar.NotifyColumnRemoved(_columns[index]);
        _columns.RemoveAt(index);
    }

    private void RemoveColumn(Column column)
    {
        int index = _columns.IndexOf(column);
        RemoveColumn(index);
    }

    private void RemoveColumn(ColumnDef columnDef)
    {
        int index = _columns.FindIndex(column => column.Worker.Def == columnDef);
        RemoveColumn(index);
    }

    private sealed class Column
    {
        public float Width;
        public readonly ColumnWorker<TObject> Worker;
        public bool IsWidthSetManually;

        private readonly Widget _titleWidget;
        private readonly float _titleWidgetWidth;
        private readonly TipSignal _tooltip;
        private readonly ObjectTable<TObject> _parent;
        private readonly FloatMenu _menu;
        private bool _isResized;
        private float _resizeWidthOffset;

        public Column(ColumnWorker<TObject> worker, TableWorker tableWorker, ObjectTable<TObject> parent)
        {
            ColumnDef def = worker.Def;
            Widget titleWidget = def.TitleWidget;
            Vector2 titleWidgetSize = titleWidget.Size;

            Worker = worker;
            _titleWidget = titleWidget;
            _titleWidgetWidth = titleWidgetSize.x;
            _tooltip = $"<i>{def.LabelCap}</i>\n\n{def.description}";
            _parent = parent;
            _menu = new FloatMenu([
                new FloatMenuOption("Remove", () => parent.RemoveColumn(this))
            ]);
        }

        public void DrawHeaderCell(Rect rect)
        {
            ColumnType columnType = Worker.Type;
            Rect titleRect = rect.ContractedByObjectTableCellPadding();
            if (columnType == ColumnType.Number)
            {
                titleRect.CutRight(out titleRect, _titleWidgetWidth);
            }
            else if (columnType == ColumnType.Boolean)
            {
                titleRect.CutMidX(out titleRect, _titleWidgetWidth);
            }

            bool isRepaint = Event.current.IsRepaint();

            if (isRepaint)
            {
                if (_parent._currentlyReorderedColumn == this)
                {
                    rect.HighlightSelected();
                }

                _titleWidget.Draw(titleRect);
            }

            if (Event.current.type == EventType.MouseDown && Mouse.IsOver(rect))
            {
                if (Event.current.shift)
                {
                    // Reset size
                    if (Event.current.clickCount > 1)
                    {
                        IsWidthSetManually = false;
                    }
                    else // Resize start 
                    {
                        _isResized = true;
                        IsWidthSetManually = true;
                        _resizeWidthOffset = rect.xMax - Event.current.mousePosition.x;
                    }
                }
                else // Reorder start
                {
                    _parent._currentlyReorderedColumn = this;
                }
            }

            if (_isResized)
            {
                if (Event.current.type == EventType.MouseDrag)// Do resize
                {
                    Width = UI.GUIToScreenPoint(Event.current.mousePosition).x - UI.GUIToScreenPoint(rect.position).x + _resizeWidthOffset;
                    if (Width < RowHeight)
                    {
                        Width = RowHeight;
                    }
                }
                else if (Event.current.rawType == EventType.MouseUp || Event.current.shift == false)// Resize stop
                {
                    _isResized = false;
                }
            }
            else if (
                OriginalEventUtility.EventType == EventType.MouseDrag
                && _parent._currentlyReorderedColumn != null
                && _parent._currentlyReorderedColumn != this
            ) // Do reorder
            {
                float mouseScreenX = UI.GUIToScreenPoint(Event.current.mousePosition).x;

                float leftHalfX = UI.GUIToScreenPoint(new Vector2(rect.x, 0f)).x;
                float leftHalfXmax = UI.GUIToScreenPoint(new Vector2(rect.x + rect.width / 2f, 0f)).x;
                bool mouseIsOverLeftHalf = mouseScreenX > leftHalfX && mouseScreenX < leftHalfXmax;

                float rightHalfX = UI.GUIToScreenPoint(new Vector2(rect.x + rect.width / 2f, 0f)).x;
                float rightHalfXmax = UI.GUIToScreenPoint(new Vector2(rect.xMax, 0f)).x;
                bool mouseIsOverRightHalf = mouseScreenX > rightHalfX && mouseScreenX < rightHalfXmax;

                if (mouseIsOverLeftHalf)
                {
                    _parent._guiAction = () =>
                    {
                        int reorderedColumnIndex = _parent._columns.IndexOf(_parent._currentlyReorderedColumn);
                        bool reorderedColumnIsPinned = reorderedColumnIndex < _parent._leftColumnsCount;
                        _parent._columns.RemoveAt(reorderedColumnIndex);
                        if (reorderedColumnIsPinned)
                        {
                            _parent._leftColumnsCount--;
                        }

                        int thisColumnIndex = _parent._columns.IndexOf(this);
                        bool thisColumnIsPinned = thisColumnIndex < _parent._leftColumnsCount;
                        _parent._columns.Insert(thisColumnIndex, _parent._currentlyReorderedColumn);
                        if (thisColumnIsPinned)
                        {
                            _parent._leftColumnsCount++;
                        }
                    };
                }
                else if (mouseIsOverRightHalf)
                {
                    _parent._guiAction = () =>
                    {
                        int reorderedColumnIndex = _parent._columns.IndexOf(_parent._currentlyReorderedColumn);
                        bool reorderedColumnIsPinned = reorderedColumnIndex < _parent._leftColumnsCount;
                        _parent._columns.RemoveAt(reorderedColumnIndex);
                        if (reorderedColumnIsPinned)
                        {
                            _parent._leftColumnsCount--;
                        }

                        int thisColumnIndex = _parent._columns.IndexOf(this);
                        bool thisColumnIsPinned = thisColumnIndex < _parent._leftColumnsCount;
                        _parent._columns.Insert(thisColumnIndex + 1, _parent._currentlyReorderedColumn);
                        if (thisColumnIsPinned)
                        {
                            _parent._leftColumnsCount++;
                        }
                    };
                }
            }
            else if (Event.current.rawType == EventType.MouseUp)// Reorder stop
            {
                _parent._currentlyReorderedColumn = null;
            }

            // Pinning/Menu
            if (rect.ButtonGhostly())
            {
                if (Event.current.control && Event.current.IsLMB())
                {
                    HandlePin();
                }
                else if (Event.current.IsRMB())
                {
                    _menu.Open();
                }
            }

            rect.Tip(_tooltip);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePin()
        {
            int index = _parent._columns.IndexOf(this);
            if (index > _parent._leftColumnsCount - 1)
            {
                _parent._guiAction = () => _parent.PinColumn(index);
            }
            else
            {
                _parent._guiAction = () => _parent.UnpinColumn(index);
            }
        }

        public void RecalcWidth(List<int> rows)
        {
            Width = Mathf.Max(_titleWidgetWidth, Worker.GetWidth(rows)) + GUIStyles.TableCell.PadHor * 2f;
        }
    }
}
