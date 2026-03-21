using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;
using UnityEngine.UIElements;
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
            Rect titleRect = rect.ContractedByObjectTableCellPadding();
            float titleWidgetWidth = _titleWidgetWidth;
            float widthDiff = titleRect.width - titleWidgetWidth;
            titleRect.width = titleWidgetWidth;
            ColumnType columnType = Worker.Type;
            if (columnType == ColumnType.Number)
            {
                titleRect.x += widthDiff;
            }
            else if (columnType == ColumnType.Boolean)
            {
                titleRect.x += widthDiff / 2f;
            }

            _titleWidget.Draw(titleRect);

            Event currentEvent = Event.current;
            bool mouseIsOverRect = Mouse.IsOver(rect);

            // Manual resizing
            if (mouseIsOverRect && currentEvent.shift)
            {
                if (currentEvent.clickCount == 2)
                {
                    IsWidthSetManually = false;
                }
                else if (_parent._currentlyResizedColumn == null && currentEvent.type == EventType.MouseDown)
                {
                    _parent._currentlyResizedColumn = this;
                    IsWidthSetManually = true;
                }

                if (_parent._currentlyResizedColumn == this && currentEvent.type == EventType.MouseDrag)
                {
                    Width = Mathf.Max(Width + currentEvent.delta.x, RowHeight);
                }
            }

            // Reordering
            if (mouseIsOverRect && currentEvent.type == EventType.MouseDrag && _parent._currentlyResizedColumn == null && _parent._currentlyReorderedColumn == null)
            {
                _parent._currentlyReorderedColumn = this;
            }

            if (_parent._currentlyReorderedColumn == this && currentEvent.type == EventType.Repaint)
            {
                rect.HighlightSelected();
            }

            if (currentEvent.type == EventType.MouseDrag && _parent._currentlyReorderedColumn != null && _parent._currentlyReorderedColumn != this)
            {
                // TODO: Check if a column on the left/right is already _parent._currentlyReorderedColumn
                if (Mouse.IsOver(rect.LeftHalf()))
                {
                    _parent._guiAction = () =>
                    {
                        _parent._columns.Remove(_parent._currentlyReorderedColumn);
                        int thisColumnIndex = _parent._columns.IndexOf(this);
                        _parent._columns.Insert(thisColumnIndex, _parent._currentlyReorderedColumn);
                    };
                }
                else if (Mouse.IsOver(rect.RightHalf()))
                {
                    _parent._guiAction = () =>
                    {
                        _parent._columns.Remove(_parent._currentlyReorderedColumn);
                        int thisColumnIndex = _parent._columns.IndexOf(this);
                        _parent._columns.Insert(thisColumnIndex + 1, _parent._currentlyReorderedColumn);
                    };
                }
            }

            if (currentEvent.type == EventType.MouseUp)
            {
                _parent._currentlyResizedColumn = null;
                _parent._currentlyReorderedColumn = null;
            }

            // Pinning/Menu
            if (rect.ButtonGhostly())
            {
                if (currentEvent.control && Event.current.IsLMB())
                {
                    HandlePinning();
                }
                else if (Event.current.IsRMB())
                {
                    _menu.Open();
                }
            }

            rect.Tip(_tooltip);
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePinning()
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
