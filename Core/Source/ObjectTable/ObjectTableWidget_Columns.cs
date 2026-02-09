using System;
using System.Collections.Generic;
using Stats.ObjectTable.Cells;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    private sealed class Column
    {
        private readonly ColumnDef Def;
        private readonly IColumnWorker<TObject> Worker;
        private readonly CellStyleType CellStyle;
        public float Width { get; set; }
        public Widget Title => Def.Title;
        public TipSignal Tooltip { get; }
        public bool IsVisible
        {
            get;
            private set
            {
                field = value;

                OnChange?.Invoke();
            }
        } = true;
        public bool IsPinned
        {
            get;
            private set
            {
                field = value;

                OnChange?.Invoke();
            }
        }
        public bool IsHot => throw new NotImplementedException();
        public bool HasActiveFilters => throw new NotImplementedException();
        public event Action? OnChange;
        public Column(ColumnDef def, IColumnWorker<TObject> worker, TableWorker tableWorker, bool isPinned)
        {
            CellDescriptor cellDescriptor = worker.GetCellDescriptor(tableWorker);

            Def = def;
            Worker = worker;
            Tooltip = $"<i>{def.LabelCap}</i>\n\n{def.Description}";
            IsPinned = isPinned;
        }
        public Cell MakeCell(TObject @object) => Worker.MakeCell(@object);
        public Widget GetHeaderCell()
        {
            Widget columnTitle = Title;

            if (CellStyle == CellStyleType.Number)
            {
                columnTitle = new SingleElementContainer(columnTitle.PaddingRel(1f, 0f, 0f, 0f));
            }
            else if (CellStyle == CellStyleType.Boolean)
            {
                columnTitle = new SingleElementContainer(columnTitle.PaddingRel(0.5f, 0f));
            }

            return columnTitle
                .TextAnchor((TextAnchor)CellStyle)
                .PaddingAbs(CellPadHor, CellPadVer)
                .ToButtonGhostly(() =>
                {
                    if (Event.current.control)
                    {
                        IsPinned = !IsPinned;
                    }
                })
                .Tooltip(Tooltip);
        }
        //public IEnumerable<ColumnPart> GetParts()
        //{
        //    return Worker.GetCellDescriptor(TODO);
        //}
        public void ToggleVisibility()
        {
            IsVisible = !IsVisible;
        }
    }

    private sealed class ColumnCollection(int capacity)
    {
        private readonly List<Column> All = new(capacity);
        private readonly List<Column> _Pinned_Visible = new(capacity);
        public IReadOnlyCollection<Column> Pinned_Visible => _Pinned_Visible;
        private readonly List<Column> _Unpinned_Visible = new(capacity);
        public IReadOnlyCollection<Column> Unpinned_Visible => _Unpinned_Visible;
        private readonly List<Column> _Hot_HasActiveFilters = new(capacity);
        public IReadOnlyCollection<Column> Hot_HasActiveFilters => _Hot_HasActiveFilters;
        private readonly List<Column> _Hot_DoesntHaveActiveFilters_Visible = new(capacity);
        public IReadOnlyCollection<Column> Hot_DoesntHaveActiveFilters_Visible => _Hot_DoesntHaveActiveFilters_Visible;
        private bool IsStale = true;
        public void Add(Column column)
        {
            column.OnChange += () => IsStale = true;

            All.Add(column);
        }
        public void Update()
        {
            if (IsStale)
            {
                UpdatePinnedColumns();

                IsStale = false;
            }

            if (Event.current.type == EventType.Layout)
            {
                UpdateHotColumns();
            }
        }
        private void UpdatePinnedColumns()
        {
            _Pinned_Visible.Clear();
            _Unpinned_Visible.Clear();

            foreach (Column column in All)
            {
                if (column.IsVisible)
                {
                    if (column.IsPinned)
                    {
                        _Pinned_Visible.Add(column);
                    }
                    else
                    {
                        _Unpinned_Visible.Add(column);
                    }
                }
            }
        }
        private void UpdateHotColumns()
        {
            _Hot_HasActiveFilters.Clear();
            _Hot_DoesntHaveActiveFilters_Visible.Clear();

            foreach (Column column in All)
            {
                if (column.HasActiveFilters)
                {
                    _Hot_HasActiveFilters.Add(column);
                }
                else if (column is { IsHot: true, IsVisible: true })
                {
                    _Hot_DoesntHaveActiveFilters_Visible.Add(column);
                }
            }
        }
    }
}
