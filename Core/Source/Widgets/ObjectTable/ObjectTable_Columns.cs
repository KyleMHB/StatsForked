using System.Collections.Generic;
using UnityEngine;
using Verse;
using static Stats.IColumnWorker;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private void UpdateCachedColumns()
    {
        ColumnsVisible.Clear();
        ColumnsVisiblePinned.Clear();
        ColumnsVisibleUnpinned.Clear();

        foreach (var column in Columns)
        {
            if (column.IsVisible)
            {
                ColumnsVisible.Add(column);

                if (column.IsPinned)
                {
                    ColumnsVisiblePinned.Add(column);
                }
                else
                {
                    ColumnsVisibleUnpinned.Add(column);
                }
            }
        }

        DoUpdateCachedColumns = false;
    }

    private sealed class Column
    {
        public bool IsPinned { get; set; }
        public float Width { get; set; }
        public bool IsVisible { get; private set; } = true;
        public ColumnDef Def => Worker.Def;
        private readonly IColumnWorker<TObject> Worker;
        private readonly ObjectTable<TObject> Parent;
        public TextAnchor CellTextAnchor => (TextAnchor)Worker.CellStyle;
        public TipSignal Tooltip { get; }
        private readonly List<Cell.IRefreshable> CellsToRefresh = new(InitialRowCapacity);
        public bool NeedsRefresh => CellsToRefresh.Count > 0;
        public Column(IColumnWorker<TObject> worker, ObjectTable<TObject> parent)
        {
            Worker = worker;
            Parent = parent;
            Tooltip = $"<i>{Def.LabelCap}</i>\n\n{Def.Description}";
        }
        public Cell GetCell(TObject @object)
        {
            var cell = Worker.GetCell(@object);

            if (cell is Cell.IRefreshable refreshableCell)
            {
                CellsToRefresh.Add(refreshableCell);
            }

            return cell;
        }
        public Widget GetHeaderCell()
        {
            var columnTitle = Def.Title;

            if (Worker.CellStyle == CellStyleType.Number)
            {
                columnTitle = new SingleElementContainer(columnTitle.PaddingRel(1f, 0f, 0f, 0f));
            }
            else if (Worker.CellStyle == CellStyleType.Boolean)
            {
                columnTitle = new SingleElementContainer(columnTitle.PaddingRel(0.5f, 0f));
            }

            var cellWidget = columnTitle
            .PaddingAbs(CellPadHor, CellPadVer)
            .Background(rect =>
            {
                if (Parent.SortColumn != this)
                    return;

                if (Parent.SortDirection == SortDirectionAscending)
                {
                    rect.y = rect.yMax - SortIndicatorHeight;
                    rect.height = SortIndicatorHeight;
                }
                else
                {
                    rect.height = SortIndicatorHeight;
                }

                Verse.Widgets.DrawBoxSolid(rect, SortIndicatorColor);
            })
            .ToButtonGhostly(() =>
            {
                if (Event.current.control)
                {
                    IsPinned = !IsPinned;

                    Parent.DoUpdateCachedColumns = true;
                }
                else
                {
                    if (Parent.SortColumn == this)
                    {
                        Parent.SortDirection *= -1;
                    }
                    else
                    {
                        Parent.SortColumn = this;
                    }

                    Parent.DoSort = true;
                }
            })
            .Tooltip(Tooltip);

            return cellWidget;
        }
        public IEnumerable<ObjectProp> GetObjectProps(TableWorker<TObject> tableWorker)
        {
            return Worker.GetObjectProps(tableWorker);
        }
        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;

            Parent.DoResize = true;
            Parent.DoUpdateCachedColumns = true;
        }
        public bool RefreshCells()
        {
            var wasUpdated = false;

            foreach (var cell in CellsToRefresh)
            {
                wasUpdated |= cell.Refresh();
            }

            return wasUpdated;
        }
        public void DisposeOfCell(Cell cell)
        {
            CellsToRefresh.Remove((Cell.IRefreshable)cell);
        }
    }
}
