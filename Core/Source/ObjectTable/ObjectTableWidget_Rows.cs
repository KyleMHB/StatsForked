using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ObjectTable.Cells;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    // TODO: If the order of rows does not matter (because we'll sort them afterwards anyway)
    // we can remove the row without moving adjacent rows by replacing it with last row (and removing it).
    private void PinRow(int index)
    {
        List<Row> unpinnedRows = _unpinnedRows;

        _pinnedRows.Add(unpinnedRows[index]);
        unpinnedRows.RemoveAt(index);
    }

    private void UnpinRow(int index)
    {
        List<Row> pinnedRows = _pinnedRows;

        _unpinnedRows.Add(pinnedRows[index]);
        pinnedRows.RemoveAt(index);
    }

    private sealed class Row
    {
        public float Height;
        public readonly Cell[] Cells;
        //public readonly TObject Object;

        private readonly int _cellsCount;
        private bool _isHovered = false;
        private readonly ObjectTableWidget<TObject> _parent;

        public Row(List<Column> columns, TObject @object, ObjectTableWidget<TObject> parent) : base()
        {
            // TODO: We can find all columns that are compatible with the table and create cells array
            // the size of count of these columns so we won't have to resize it when we will add columns later.
            int columnsCount = columns.Count;
            Cell[] cells = new Cell[columnsCount];

            for (int i = 0; i < columnsCount; i++)
            {
                Column column = columns[i];
                Cell cell = column.MakeCell(@object);

                cells[column.CellIndex] = cell;
            }

            Cells = cells;
            _cellsCount = columnsCount;
            _parent = parent;
            //Object = @object;
        }

        public void Draw(Rect rect, List<Column> columns, float offsetX, int index)
        {
            bool mouseIsOverRect = Mouse.IsOver(rect);

            if (mouseIsOverRect)
            {
                _isHovered = true;
            }

            if (Event.current.type == EventType.Repaint)
            {
                // _isHovered may be true even if mouse is not over rect.
                if (_isHovered)
                {
                    Verse.Widgets.DrawHighlight(rect);
                }
                else if (index % 2 == 0)
                {
                    Verse.Widgets.DrawLightHighlight(rect);
                }
            }

            _isHovered = mouseIsOverRect;

            float viewportRightBoundary = rect.width;

            rect.x = -offsetX;

            Cell[] cells = Cells;

            for (int i = 0; i < columns.Count; i++)
            {
                Column column = columns[i];

                rect.width = column.Width;

                float cellRightBoundary = rect.xMax;

                if (cellRightBoundary > 0f)
                {
                    Cell cell = cells[column.CellIndex];

                    try
                    {
                        cell.Draw(rect);
                    }
                    catch
                    {
                        // TODO: ?
                    }
                }

                if (cellRightBoundary >= viewportRightBoundary)
                {
                    break;
                }

                rect.x = cellRightBoundary;
            }

            // This must go after cells to not interfere with their GUI events.
            if (mouseIsOverRect && Event.current is { control: true, type: EventType.MouseDown })
            {
                HandlePinning(index);
            }
        }

        [MethodImpl(MethodImplOptions.NoInlining)]
        private void HandlePinning(int index)
        {
            List<Row> pinnedRows = _parent._pinnedRows;

            if (index > pinnedRows.Count - 1 || pinnedRows[index] != this)
            {
                _parent._guiAction = () => _parent.PinRow(index);
            }
            else
            {
                _parent._guiAction = () => _parent.UnpinRow(index);
            }
        }

        public void Resize()
        {
            float height = 0f;
            Cell[] cells = Cells;
            int cellsCount = _cellsCount;

            for (int i = 0; i < cellsCount; i++)
            {
                Cell cell = cells[i];
                float cellHeight = cell.Size.y;

                if (height < cellHeight)
                {
                    height = cellHeight;
                }
            }

            Height = height;
        }
    }
}
