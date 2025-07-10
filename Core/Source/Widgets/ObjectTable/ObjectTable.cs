using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal abstract class ObjectTable
{
    public abstract void Draw(Rect rect);
    public abstract void ResetFilters();
    public abstract TableFilterMode FilterMode { get; set; }
    public abstract void ToggleFilterMode();
    public abstract event Action<TableFilterMode> OnFilterModeChange;

    public enum TableFilterMode
    {
        AND = 0,
        OR = 1,
    }
}

internal sealed partial class ObjectTable<TObject> : ObjectTable
{
    public ObjectTable(List<ColumnWorker<TObject>> columnWorkers, IEnumerable<TObject> objects)
    {
        // Columns
        var columnsCount = columnWorkers.Count;
        var columns = new Column[columnsCount];

        for (int i = 0; i < columnsCount; i++)
        {
            var columnWorker = columnWorkers[i];

            columns[i] = new Column(columnWorker, i == 0, this);
        }

        var sortColumn = columns[0];
        const int sorDirection = SortDirectionAscending;
        // TODO: Maybe the constructor should accept list from the beginning.
        var objectArr = objects.ToArray();
        // This is a bit of a hack. We rely on default sort direction being just what we need.
        Array.Sort(objectArr, sortColumn.Worker.Compare);

        // Body rows
        var bodyRows = new RowCollection<ObjectRow>(objectArr.Length);

        foreach (var @object in objectArr)
        {
            var row = new ObjectRow(columns, @object);

            for (int i = 0; i < columnsCount; i++)
            {
                var column = columns[i];

                try
                {
                    row[i] = column.Worker.GetTableCellWidget(@object)
                    ?.PaddingAbs(CellPadHor, CellPadVer);
                }
                catch (Exception e)
                {
                    row[i] = new Label("!!!")
                    .Color(Color.red.ToTransparent(0.5f))
                    .TextAnchor(TextAnchor.LowerCenter)
                    .PaddingAbs(CellPadHor, CellPadVer)
                    .Tooltip(e.Message);
                }
            }

            bodyRows.Add(row);
        }

        // Headers
        var headerRows = new RowCollection<Row>(1);
        var columnTitlesRow = new ColumnTitlesRow(columns);

        for (int i = 0; i < columnsCount; i++)
        {
            var column = columns[i];

            if (column.IsVisible == false)
            {
                continue;
            }

            void drawSortIndicator(Rect rect)
            {
                if (SortColumn == column)
                {
                    if (SortDirection == SortDirectionAscending)
                    {
                        rect.y = rect.yMax - SortIndicatorHeight;
                        rect.height = SortIndicatorHeight;
                    }
                    else
                    {
                        rect.height = SortIndicatorHeight;
                    }

                    Verse.Widgets.DrawBoxSolid(rect, SortIndicatorColor);
                }
            }
            void handleCellClick()
            {
                if (Event.current.control)
                {
                    column.IsPinned = !column.IsPinned;
                }
                else
                {
                    SortAllRowsByColumn(column);
                }
            }

            var columnDef = column.Worker.ColumnDef;
            var columnTitleWidget = columnDef.Title;

            if (column.Worker.CellStyle == ColumnCellStyle.Number)
            {
                columnTitleWidget = new SingleElementContainer(columnTitleWidget.PaddingRel(1f, 0f, 0f, 0f));
            }
            else if (column.Worker.CellStyle == ColumnCellStyle.Boolean)
            {
                columnTitleWidget = new SingleElementContainer(columnTitleWidget.PaddingRel(0.5f, 0f));
            }

            // Title
            columnTitlesRow[i] = columnTitleWidget
            .PaddingAbs(CellPadHor, CellPadVer)
            .Background(drawSortIndicator)
            .ToButtonGhostly(handleCellClick, GetColumnDefDescriptionFull(columnDef));
        }

        headerRows.Add(columnTitlesRow);

        // Column settings
        var columnLabels = new Widget[columnsCount];
        var maxColumnLabelWidth = 0f;

        for (int i = 0; i < columnsCount; i++)
        {
            var column = columns[i];

            if (column.IsVisible == false)
            {
                continue;
            }

            var columnTitleWidget = column.Worker.ColumnDef.Title;
            // TODO: This is a hack to prevent icons from stretching out.
            if (columnTitleWidget is InlineTexture)
            {
                columnTitleWidget = new SingleElementContainer(columnTitleWidget);
            }

            var columnLabel = new HorizontalContainer([
                new EmptyWidget()
                .SizeAbs(Text.LineHeight)
                .Background(rect => {
                    if (Event.current.type == EventType.Repaint) {
                        if (column.IsVisible) {
                            Verse.Widgets.DrawTextureFitted(rect, Verse.Widgets.CheckboxOnTex, 1f);
                        }
                        else
                        {
                            Verse.Widgets.DrawTextureFitted(rect, Verse.Widgets.CheckboxOffTex, 1f);
                        }
                    }
                })
                .ToButtonGhostly(column.Toggle),

                columnTitleWidget
                .PaddingAbs(Globals.GUI.PadSm, 0f)
                .WidthIncRel(1f)
                .ToButtonGhostly(() => {
                    if (Event.current.control)
                    {
                        column.IsPinned = !column.IsPinned;
                    }
                    else
                    {
                        SortAllRowsByColumn(column);
                    }
                }),
            ], Globals.GUI.PadSm, true);
            columnLabels[i] = columnLabel;
            maxColumnLabelWidth = Mathf.Max(maxColumnLabelWidth, columnLabel.GetSize().x);
        }

        var filters = new List<Widget>(columnsCount);

        for (int i = 0; i < columnsCount; i++)
        {
            var column = columns[i];

            if (column.IsVisible == false)
            {
                continue;
            }

            var filterWidget = column.Worker.GetFilterWidget(objectArr);
            filterWidget.OnChange += filterWidget => HandleFilterChange(filterWidget, column);
            Widget columnRow = new HorizontalContainer([
                columnLabels[i]
                .WidthAbs(maxColumnLabelWidth)
                .HeightRel(1f)
                .Tooltip(GetColumnDefDescriptionFull(column.Worker.ColumnDef)),

                filterWidget
                .WidthIncRel(1f),
            ], Globals.GUI.PadSm, true)
            .PaddingAbs(Globals.GUI.PadXs)
            .WidthRel(1f)
            .HoverBackground(TexUI.HighlightTex);

            if (filters.Count % 2 == 0)
            {
                columnRow = columnRow.Background(Verse.Widgets.LightHighlight);
            }

            filters.Add(columnRow);
        }

        // Finalize
        Columns = columns;
        SortColumn = sortColumn;
        SortDirection = sorDirection;
        HeaderRows = headerRows;
        PinnedRows = new(10);
        UnfilteredBodyRows = bodyRows;
        FilteredBodyRows = new(bodyRows);
        ColumnsTabWidget = new VerticalContainer(filters).PaddingAbs(Globals.GUI.PadSm);
        ActiveFilters = new HashSet<FilterWidget<TObject>>(columnsCount);
        _FilterMode = TableFilterMode.AND;
        ObjectMatchesFilters = ObjectFilterMatchFuncAND;
        ShouldApplyFilters = false;
    }
    private static string GetColumnDefDescriptionFull(ColumnDef columnDef)
    {
        return $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.Description}";
    }
}
