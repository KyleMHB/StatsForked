using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
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
        public Widget Title => Def.Title;
        private readonly ColumnDef Def;
        private readonly IColumnWorker<TObject> Worker;
        private readonly ObjectTableWidget<TObject> Parent;
        public TextAnchor CellTextAnchor => (TextAnchor)Worker.CellStyle;
        public TipSignal Tooltip { get; }
        private readonly List<IRefreshableCell> CellsToRefresh = new(InitialRowCapacity);
        public bool NeedsRefresh => CellsToRefresh.Count > 0;
        public Column(ColumnDef def, IColumnWorker<TObject> worker, ObjectTableWidget<TObject> parent)
        {
            Def = def;
            Worker = worker;
            Parent = parent;
            Tooltip = $"<i>{def.LabelCap}</i>\n\n{def.Description}";
        }
        public Cell GetCell(TObject @object)
        {
            var cell = Worker.GetCell(@object);

            if (cell is IRefreshableCell refreshableCell)
            {
                CellsToRefresh.Add(refreshableCell);
            }

            return cell;
        }
        public Widget GetHeaderCell()
        {
            var columnTitle = Title;

            if (Worker.CellStyle == IColumnWorker.CellStyleType.Number)
            {
                columnTitle = new SingleElementContainer(columnTitle.PaddingRel(1f, 0f, 0f, 0f));
            }
            else if (Worker.CellStyle == IColumnWorker.CellStyleType.Boolean)
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
        public IEnumerable<ColumnPart> GetParts()
        {
            return Worker.GetCellDescriptor();
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
            if (cell is IRefreshableCell refreshableCell)
            {
                CellsToRefresh.Remove(refreshableCell);
            }
        }
    }
}
