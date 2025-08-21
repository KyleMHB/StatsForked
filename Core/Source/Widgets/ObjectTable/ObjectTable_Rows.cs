using System.Collections.Generic;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    private void PinRow(ObjectRow row)
    {
        PinnedRows.Add(row);
        UnpinnedRows.Remove(row);
    }
    private void UnpinRow(ObjectRow row)
    {
        PinnedRows.Remove(row);
        UnpinnedRows.Add(row);
    }

    private abstract class Row
    {
        public float Height;
        public bool IsVisible = true;
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
                {
                    break;
                }

                rect.width = column.Width + cellExtraWidth;
                if (rect.xMax > 0f)
                {
                    var cell = GetCell(column);

                    if (cell != null)
                    {
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
                }

                rect.x = rect.xMax;
            }

            return false;
        }
        protected abstract Widget? GetCell(ColumnWorker<TObject> column);
        public void Resize(List<ColumnWorker<TObject>> columns)
        {
            Height = 0f;

            foreach (var column in columns)
            {
                var cell = GetCell(column);

                if (cell != null)
                {
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
            }
        }
    }

    private sealed class ColumnTitlesRow : Row
    {
        public ColumnTitlesRow(List<ColumnWorker<TObject>> columns, ObjectTable<TObject> parent) : base()
        {
            foreach (var column in columns)
            {
                column.InitHeaderCell(parent);
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
            }
        }
        protected override Widget? GetCell(ColumnWorker<TObject> column)
        {
            return column.HeaderCell;
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
    }

    private sealed class ObjectRow : Row
    {
        public readonly TObject Object;
        private bool IsHovered = false;
        public ObjectRow(List<ColumnWorker<TObject>> columns, TObject @object) : base()
        {
            Object = @object;

            foreach (var column in columns)
            {
                column.InitCell(@object);
            }
        }
        protected override Widget? GetCell(ColumnWorker<TObject> column)
        {
            return column.GetCellWidget(Object);
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

            if (Event.current.type == EventType.Repaint)
            {
                if (mouseIsOverRect)
                {
                    IsHovered = true;
                }

                // IsHovered may be true even if mouse is not over rect.
                if (IsHovered)
                {
                    Verse.Widgets.DrawHighlight(rect);
                }
                else if (index % 2 == 0)
                {
                    Verse.Widgets.DrawLightHighlight(rect);
                }

                if (mouseIsOverRect == false)
                {
                    IsHovered = false;
                }
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
    }
}
