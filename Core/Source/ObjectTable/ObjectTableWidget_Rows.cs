using System;
using System.Collections.Generic;
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
        public readonly List<Cell> Cells;
        //public readonly TObject Object;

        private bool _isHovered = false;
        private readonly ObjectTableWidget<TObject> _parent;

        public Row(List<Column> columns, TObject @object, ObjectTableWidget<TObject> parent) : base()
        {
            List<Cell> cells = new(columns.Count);

            for (int i = 0; i < columns.Count; i++)
            {
                Column column = columns[i];
                Cell cell = column.MakeCell(@object);

                cells[i] = cell;
            }

            Cells = cells;
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
                // IsHovered may be true even if mouse is not over rect.
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

            float xMax = rect.width;
            rect.x = -offsetX;

            for (int i = 0; i < columns.Count; i++)
            {
                if (rect.x >= xMax)
                {
                    break;
                }

                Column column = columns[i];

                rect.width = column.Width;

                if (rect.xMax > 0f)
                {
                    Cell cell = Cells[column.CellIndex];

                    try
                    {
                        cell.Draw(rect);
                    }
                    catch
                    {
                        // TODO: ?
                    }
                }

                rect.x = rect.xMax;
            }

            // This must go after cells to not interfere with their GUI events.
            if (mouseIsOverRect && Event.current is { control: true, type: EventType.MouseDown })
            {
                HandlePinning(index);
            }
        }

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
            List<Cell> cells = Cells;

            for (int i = 0; i < cells.Count; i++)
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

    //private sealed class ColumnTitlesRow : Row<Widget>
    //{
    //    public ColumnTitlesRow(List<Column> columns) : base()
    //    {
    //        Dictionary<Column, Widget> cells = new(columns.Count);
    //        float height = 0f;

    //        foreach (Column column in columns)
    //        {
    //            Widget cell = column.MakeHeaderCell();
    //            float cellHeight = cell.GetSize().y;

    //            if (height < cellHeight)
    //            {
    //                height = cellHeight;
    //            }

    //            cells[column] = cell;
    //        }

    //        this.cells = cells;
    //        this.height = height;
    //    }
    //    public override void Draw(
    //        Rect rect,
    //        List<Column> columns,
    //        float offsetX,
    //        float cellExtraWidth,
    //        int index
    //    )
    //    {
    //        Verse.Widgets.DrawHighlight(rect);

    //        base.Draw(rect, columns, offsetX, cellExtraWidth, index);
    //    }
    //}
}
