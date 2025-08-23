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
        row.IsVisible = ObjectMatchesFilters(row.Object, ActiveFilters);
        SortRows(UnpinnedRows);
    }

    private abstract class Row
    {
        public float Height;
        public bool IsVisible = true;
        protected abstract Dictionary<ColumnWorker<TObject>, Widget> Cells { get; }
        public Row()
        {
        }
        public virtual bool Draw(
            Rect rect,
            List<ColumnWorker<TObject>> columns,
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
        public abstract float Resize(List<ColumnWorker<TObject>> columns);
    }

    private sealed class ColumnTitlesRow : Row
    {
        protected override Dictionary<ColumnWorker<TObject>, Widget> Cells { get; }
        public ColumnTitlesRow(List<ColumnWorker<TObject>> columns, ObjectTable<TObject> parent) : base()
        {
            var cells = new Dictionary<ColumnWorker<TObject>, Widget>(columns.Count);

            foreach (var column in columns)
            {
                var cell = column.InitHeaderCell(parent);

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
            List<ColumnWorker<TObject>> columns,
            float offsetX,
            float cellExtraWidth,
            int index
        )
        {
            Verse.Widgets.DrawHighlight(rect);

            return base.Draw(rect, columns, offsetX, cellExtraWidth, index);
        }
        public override float Resize(List<ColumnWorker<TObject>> columns)
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

    private sealed class ObjectRow : Row
    {
        protected override Dictionary<ColumnWorker<TObject>, Widget> Cells { get; }
        public readonly TObject Object;
        private bool IsHovered = false;
        public ObjectRow(List<ColumnWorker<TObject>> columns, TObject @object) : base()
        {
            var cells = new Dictionary<ColumnWorker<TObject>, Widget>(columns.Count);

            foreach (var column in columns)
            {
                cells[column] = column.InitCell(@object);
            }

            Cells = cells;
            Object = @object;
        }
        public override bool Draw(
            Rect rect,
            List<ColumnWorker<TObject>> columns,
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
        public override float Resize(List<ColumnWorker<TObject>> columns)
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
    }
}
