using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class ColumnWorker
{
    internal bool IsPinned { get; set; }
    internal float Width { get; set; }
    internal bool IsVisible { get; private set; } = true;
    internal TipSignal Tooltip { get; }
    // Why is this not in ColumnDef?
    //
    // This part of a column representation is governed by column worker,
    // because only column worker knows what type of data it encapsulates.
    protected CellStyleType CellStyle { get; }
    internal TextAnchor CellTextAnchor { get; }
    public ColumnDef ColumnDef { get; }
    internal event Action? OnVisibilityChange;
    protected event Action? OnRefresh;
    protected ColumnWorker(ColumnDef columnDef, CellStyleType cellStyle)
    {
        ColumnDef = columnDef;
        CellStyle = cellStyle;
        CellTextAnchor = (TextAnchor)cellStyle;
        Tooltip = $"<i>{ColumnDef.LabelCap}</i>\n\n{ColumnDef.Description}";
    }
    internal void ToggleVisibility()
    {
        IsVisible = !IsVisible;
        OnVisibilityChange?.Invoke();
    }
    internal void RefreshCells()
    {
        OnRefresh?.Invoke();
    }

    protected enum CellStyleType
    {
        Number = TextAnchor.LowerRight,
        String = TextAnchor.LowerLeft,
        Boolean = TextAnchor.LowerCenter,
    }

    // TODO: Shouldn't this be in the table, since it's what the table says it works with and
    // a column worker only satisfies table's requirements.
    public readonly record struct ObjectProp(Widget Label, FilterWidget FilterWidget);
}

public abstract class ColumnWorker<TObject> : ColumnWorker
{
    private static readonly Color SortIndicatorColor = Color.yellow.ToTransparent(0.3f);
    private const float SortIndicatorHeight = 5f;
    internal event Action? OnHeaderCellClick;
    protected ColumnWorker(ColumnDef columnDef, CellStyleType cellStyle) : base(columnDef, cellStyle)
    {
    }
    internal Widget GetHeaderCell(ObjectTable<TObject> parent)
    {
        var columnTitle = ColumnDef.Title;

        if (CellStyle == CellStyleType.Number)
        {
            columnTitle = new SingleElementContainer(columnTitle.PaddingRel(1f, 0f, 0f, 0f));
        }
        else if (CellStyle == CellStyleType.Boolean)
        {
            columnTitle = new SingleElementContainer(columnTitle.PaddingRel(0.5f, 0f));
        }

        var cellWidget = columnTitle
        .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer)
        .Background(rect =>
        {
            if (parent.SortColumn != this)
                return;

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

        return cellWidget;
    }
    public abstract ObjectTable.Cell GetCell(TObject @object);
    public abstract IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> contextObjects);
}
