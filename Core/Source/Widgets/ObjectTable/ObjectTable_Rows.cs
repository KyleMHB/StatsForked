using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    private void PinRow(ObjectRow row)
    {
        PinnedRows.Add(row);
        PinnedRowsHeight += row.Height;
        UnpinnedRows.Remove(row);
        UnpinnedRowsHeight -= row.Height;
        SortRows(PinnedRows);
    }
    private void UnpinRow(ObjectRow row)
    {
        PinnedRows.Remove(row);
        PinnedRowsHeight -= row.Height;
        UnpinnedRows.Add(row);
        row.IsVisible = MatchRowCells(row.Cells, ActiveFilters);
        SortRows(UnpinnedRows);
    }

    private abstract class Row
    {
        public float Height;
        public bool IsVisible = true;
        public abstract bool Draw(
            Rect rect,
            List<ColumnWorker> columns,
            float offsetX,
            float cellExtraWidth,
            int index
        );
        public abstract float Resize(List<ColumnWorker> columns);
    }

    private abstract class Row<TCell> : Row where TCell : Widget
    {
        public abstract Dictionary<ColumnWorker, TCell> Cells { get; }
        public override bool Draw(
            Rect rect,
            List<ColumnWorker> columns,
            float offsetX,
            float cellExtraWidth,
            int index
        )
        {
            var xMax = rect.width;
            rect.x = -offsetX;

            foreach (var column in columns)
            {
                if (rect.x >= xMax)
                    break;

                rect.width = column.Width + cellExtraWidth;

                if (rect.xMax > 0f)
                {
                    var cell = Cells[column];

                    try
                    {
                        var origTextAnchor = Text.Anchor;
                        Text.Anchor = column.CellTextAnchor;

                        // Basically, relative size extensions are not allowed on table cell widgets.
                        // Saves us some CPU cycles and is pointless to do anyway.
                        var cellSize = cell.GetSize();
                        rect.height = cellSize.y;

                        cell.Draw(rect, cellSize);

                        Text.Anchor = origTextAnchor;
                    }
                    catch
                    {
                        // TODO: ?
                    }
                }

                rect.x = rect.xMax;
            }

            return false;
        }
    }

    private sealed class ColumnTitlesRow : Row<Widget>
    {
        public override Dictionary<ColumnWorker, Widget> Cells { get; }
        public ColumnTitlesRow(IReadOnlyList<ColumnWorker<TObject>> columns, ObjectTable<TObject> parent) : base()
        {
            var cells = new Dictionary<ColumnWorker, Widget>(columns.Count);

            foreach (var column in columns)
            {
                var cell = column.GetHeaderCell(parent);

                column.OnVisibilityChange += () =>
                {
                    parent.DoResize = true;
                    parent.DoUpdateCachedColumns = true;
                };
                column.OnHeaderCellClick += () =>
                {
                    if (Event.current.control)
                    {
                        column.IsPinned = !column.IsPinned;

                        parent.DoUpdateCachedColumns = true;
                    }
                    else
                    {
                        if (parent.SortColumn == column)
                        {
                            parent.SortDirection *= -1;
                        }
                        else
                        {
                            parent.SortColumn = column;
                        }

                        parent.DoSort = true;
                    }
                };

                cells[column] = cell;
            }

            Cells = cells;
        }
        public override bool Draw(
            Rect rect,
            List<ColumnWorker> columns,
            float offsetX,
            float cellExtraWidth,
            int index
        )
        {
            Verse.Widgets.DrawHighlight(rect);

            return base.Draw(rect, columns, offsetX, cellExtraWidth, index);
        }
        public override float Resize(List<ColumnWorker> columns)
        {
            Height = 0f;

            foreach (var column in columns)
            {
                var cell = Cells[column];
                var cellSize = cell.GetSize();

                column.Width = cellSize.x;

                if (Height < cellSize.y)
                {
                    Height = cellSize.y;
                }
            }

            return Height;
        }
    }

    private sealed class ObjectRow : Row<Cell>
    {
        public override Dictionary<ColumnWorker, Cell> Cells { get; }
        private bool IsHovered = false;
        public ObjectRow(List<ColumnWorker<TObject>> columns, TObject @object, ObjectTable<TObject> parent) : base()
        {
            var cells = new Dictionary<ColumnWorker, Cell>(columns.Count);

            foreach (var column in columns)
            {
                var cell = column.GetCell(@object);

                cells[column] = cell;
                cell.OnChange += () => parent.HandleCellUpdate(column);
            }

            Cells = cells;
        }
        public override bool Draw(
            Rect rect,
            List<ColumnWorker> columns,
            float offsetX,
            float cellExtraWidth,
            int index
        )
        {
            var mouseIsOverRect = Mouse.IsOver(rect);

            if (mouseIsOverRect)
            {
                IsHovered = true;
            }

            if (Event.current.type == EventType.Repaint)
            {
                // IsHovered may be true even if mouse is not over rect.
                if (IsHovered)
                {
                    Verse.Widgets.DrawHighlight(rect);
                }
                else if (index % 2 == 0)
                {
                    Verse.Widgets.DrawLightHighlight(rect);
                }
            }

            if (mouseIsOverRect == false)
            {
                IsHovered = false;
            }

            base.Draw(rect, columns, offsetX, cellExtraWidth, index);

            // This must go after cells to not interfere with their GUI events.
            if
            (
                Event.current.control
                && Event.current.type == EventType.MouseDown
                && mouseIsOverRect
            )
            {
                // Prevents flickering on pinning.
                IsHovered = false;

                return true;
            }

            return false;
        }
        public override float Resize(List<ColumnWorker> columns)
        {
            Height = 0f;

            foreach (var column in columns)
            {
                var cell = Cells[column];
                var cellSize = cell.GetSize();

                if (column.Width < cellSize.x)
                {
                    column.Width = cellSize.x;
                }

                if (Height < cellSize.y)
                {
                    Height = cellSize.y;
                }
            }

            return Height;
        }
        public int CompareToByColumn(ObjectRow row, ColumnWorker column)
        {
            // Idea: Upon sorting, if SortColumn != column, move the sort columns cell to a row,
            // so when the data updates we won't have to go through the Cells dictionary to find the cell.
            // Although, this will only optimize resorting on cell updates, not the sorting itself.
            var result = Cells[column].CompareTo(row.Cells[column]);

            if (result == 0)
            {
                result = GetHashCode().CompareTo(row.GetHashCode());
            }

            return result;
        }
    }
}
