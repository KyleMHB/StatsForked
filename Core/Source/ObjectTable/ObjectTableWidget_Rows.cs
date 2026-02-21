using System.Collections.Generic;
using Stats.ObjectTable.Cells;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    private sealed class Row
    {
        public float Height;
        public readonly List<Cell> Cells;
        public bool IsPinned;
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

        public void Draw(Rect rect, List<Column> columns, Range columnsRange, float offsetX, int index)
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

            for (int i = columnsRange.Start; i < columnsRange.End; i++)
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
                IsPinned = !IsPinned;
                _parent._rowToPinOrUnpin = this;
            }
        }

        public void Resize()
        {
            float height = 0f;

            for (int i = 0; i < Cells.Count; i++)
            {
                Cell cell = Cells[i];
                float cellHeight = cell.Size.y;

                if (height < cellHeight)
                {
                    height = cellHeight;
                }
            }

            Height = height;
        }
    }

    /// <summary>
    /// A simple int range.
    /// </summary>
    /// <param name="start">Inclusive</param>
    /// <param name="end">Exclusive</param>
    private readonly struct Range(int start, int end)
    {
        /// <summary>
        /// Inclusive
        /// </summary>
        public readonly int Start = start;
        /// <summary>
        /// Exclusive
        /// </summary>
        public readonly int End = end;
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
