using System.Collections.Generic;
using System.Runtime.CompilerServices;
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
        protected readonly List<ColumnWorker<TObject>> Columns;
        public float Height;
        public bool IsVisible = true;
        public Row(List<ColumnWorker<TObject>> columns)
        {
            Columns = columns;
        }
        public virtual bool Draw(
            Rect rect,
            float offsetX,
            bool drawPinnedColumns,
            float cellExtraWidth,
            int index
        )
        {
            var xMax = rect.width;
            rect.x = -offsetX;

            foreach (var column in Columns)
            {
                if (rect.x >= xMax)
                {
                    break;
                }

                if (column.IsPinned != drawPinnedColumns || column.IsVisible == false)
                {
                    continue;
                }

                rect.width = column.Width + cellExtraWidth;
                if (rect.xMax > 0f)
                {
                    try
                    {
                        GetCell(column)?.Draw(rect, Vector2.zero);
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
        protected abstract Widget? GetCell(ColumnWorker<TObject> column);
        public void Resize()
        {
            Height = 0f;

            foreach (var column in Columns)
            {
                if (column.IsVisible == false) continue;

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
        public ColumnTitlesRow(List<ColumnWorker<TObject>> columns, ObjectTable<TObject> parent) : base(columns)
        {
            foreach (var column in columns)
            {
                column.InitHeaderCell(parent);
                column.OnVisibilityChange += () => parent.DoResize = true;
                column.OnHeaderCellClick += () =>
                {
                    if (Event.current.control)
                    {
                        column.IsPinned = !column.IsPinned;
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
            float offsetX,
            bool drawPinnedColumns,
            float cellExtraWidth,
            int index
        )
        {
            Verse.Widgets.DrawHighlight(rect);

            return base.Draw(rect, offsetX, drawPinnedColumns, cellExtraWidth, index);
        }
    }

    private sealed class ObjectRow : Row
    {
        public readonly TObject Object;
        private bool IsHovered = false;
        public ObjectRow(List<ColumnWorker<TObject>> columns, TObject @object) : base(columns)
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
            float offsetX,
            bool drawPinnedColumns,
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

            base.Draw(rect, offsetX, drawPinnedColumns, cellExtraWidth, index);

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
