using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

// Column worker encapsulates a virtual property of a TObject (ThingAlike/GeneDef/etc).
//
// Column worker provides following abstractions:
// - GetTableCellWidget: how to display values of the property.
// - GetFilterWidget: what filter widget to use to filter by this property.
// - Compare: how to compare two property values.

public abstract class ColumnWorker<TObject>
{
    internal bool IsPinned;
    internal float Width;
    internal bool IsVisible = true;
    internal readonly TipSignal Tooltip;
    internal Widget HeaderCell;
    private static readonly Color SortIndicatorColor = Color.yellow.ToTransparent(0.3f);
    private const float SortIndicatorHeight = 5f;
    // Why is this not in ColumnDef?
    //
    // This part of a column representation is governed by column worker,
    // because only column worker knows what type of data it encapsulates.
    private ColumnCellStyle CellStyle { get; }
    protected readonly TextAnchor CellTextAnchor;
    public ColumnDef ColumnDef { get; }
    public event Action? OnChange;
    internal event Action? OnVisibilityChange;
    internal event Action? OnHeaderCellClick;
    protected ColumnWorker(ColumnDef columnDef, ColumnCellStyle cellStyle)
    {
        ColumnDef = columnDef;
        CellStyle = cellStyle;
        CellTextAnchor = (TextAnchor)cellStyle;
        HeaderCell = columnDef.Title;
        Tooltip = $"<i>{ColumnDef.LabelCap}</i>\n\n{ColumnDef.Description}";
    }
    internal void InitHeaderCell(ObjectTable<TObject> parent)
    {
        if (CellStyle == ColumnCellStyle.Number)
        {
            HeaderCell = new SingleElementContainer(HeaderCell.PaddingRel(1f, 0f, 0f, 0f));
        }
        else if (CellStyle == ColumnCellStyle.Boolean)
        {
            HeaderCell = new SingleElementContainer(HeaderCell.PaddingRel(0.5f, 0f));
        }

        HeaderCell = HeaderCell
        .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer)
        .Background(rect =>
        {
            if (parent.SortColumn != this) return;

            if (parent.SortDirection == ObjectTable<TObject>.SortDirectionAscending)
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
        .ToButtonGhostly(() => OnHeaderCellClick?.Invoke())
        .Tooltip(Tooltip);
    }
    internal abstract void InitCell(TObject @object);
    internal abstract Widget? GetCellWidget(TObject @object);
    // We pass IEnumerable<TObject> to this method, mainly so that if a column worker returns
    // one/many-to-many filter widget, it can generate a superset of all possible distinct
    // options for it.
    public abstract IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords);
    public abstract int Compare(TObject object1, TObject object2);
    internal void Toggle()
    {
        IsVisible = !IsVisible;
        OnVisibilityChange?.Invoke();
    }
    //protected void NotifyChanged()
    //{
    //    OnChange?.Invoke();
    //}
    //public virtual bool Refresh()
    //{
    //    return false;
    //}

    public readonly record struct ObjectProp(Widget Label, FilterWidget<TObject> FilterWidget);
}

public abstract class ColumnWorker<TObject, TData> : ColumnWorker<TObject>
{
    protected readonly Dictionary<TObject, Cell> Cells;
    protected ColumnWorker(ColumnDef columnDef, ColumnCellStyle cellStyle) : base(columnDef, cellStyle)
    {
        Cells = new Dictionary<TObject, Cell>(250);
    }
    protected abstract Cell GetCell(TObject @object);
    internal sealed override void InitCell(TObject @object)
    {
        Cells[@object] = GetCell(@object);
    }
    internal sealed override Widget? GetCellWidget(TObject @object)
    {
        return Cells[@object].Widget;
    }

    protected readonly record struct Cell(Widget? Widget, TData Data);
}

public enum ColumnCellStyle
{
    Number = TextAnchor.LowerRight,
    String = TextAnchor.LowerLeft,
    Boolean = TextAnchor.LowerCenter,
}
