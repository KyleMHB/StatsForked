using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal abstract class ObjectTable
{
    public abstract void Draw(Rect rect, bool showSettingsMenu);
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

        string GetColumnDefDescriptionFull(ColumnDef columnDef)
        {
            return $"<i>{columnDef.LabelCap}</i>\n\n{columnDef.Description}";
        }

        for (int i = 0; i < columnsCount; i++)
        {
            var column = columns[i];

            if (column.IsVisible == false) continue;

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
            .Background((Rect rect) =>
            {
                if (SortColumn != column) return;

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
            })
            .ToButtonGhostly(() =>
            {
                if (Event.current.control)
                {
                    column.IsPinned = !column.IsPinned;
                }
                else
                {
                    SortAllRowsByColumn(column);
                }
            }, GetColumnDefDescriptionFull(columnDef));
        }

        headerRows.Add(columnTitlesRow);

        // Column settings
        var columnSettingsTabRows = new List<Widget>(columnsCount);
        var columnLabelWidthMax = new StrongBox<float>(0f);

        Widget MakeColumnTitleRow(Widget columnTitle, Widget filterOrToggle, Column column)
        {
            return new HorizontalContainer([
                // TODO: This is a hack to prevent icons from stretching out.
                (columnTitle.Get<InlineTexture>() == null
                    ? columnTitle
                    : new SingleElementContainer(columnTitle))
                .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                .WriteWidthNowTo(ref columnLabelWidthMax.Value)
                .Foreground(rect =>
                {
                    if (Event.current.type != EventType.Repaint || column.IsVisible) return;

                    var origGUIColor = GUI.color;
                    GUI.color = Globals.GUI.TextColorSecondary;

                    Verse.Widgets.DrawLineHorizontal(rect.x, rect.y + rect.height / 2f, rect.width);

                    GUI.color = origGUIColor;
                })
                .ToButtonGhostly(column.Toggle, GetColumnDefDescriptionFull(column.Worker.ColumnDef))
                .WidthAbsShared(columnLabelWidthMax),

                filterOrToggle,
            ], 0f, true)
            .WidthRel(1f);
        }

        for (int i = 0; i < columnsCount; i++)
        {
            var column = columns[i];

            if (column.IsVisible == false) continue;

            var objectProps = column.Worker.GetObjectProps(objectArr).ToList();

            if (objectProps.Count == 0) continue;

            Widget row;

            if (objectProps.Count == 1)
            {
                var objectProp = objectProps[0];
                row = MakeColumnTitleRow(
                    objectProp.Label,
                    objectProp.FilterWidget
                    .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                    .WidthIncRel(1f),
                    column
                )
                .WidthRel(1f)
                .HoverBackground(TexUI.HighlightTex);

                objectProp.FilterWidget.OnChange += HandleFilterChange;
            }
            else
            {
                var objectPropLabelWidthMax = new StrongBox<float>(0f);
                var toggle = new Observable<bool>(true);
                row = new VerticalContainer([
                    MakeColumnTitleRow(
                        column.Worker.ColumnDef.Title,
                        new Label(toggle.Map(state => state
                            ? $"<i>Hide ({objectProps.Count}) filters</i>"
                            : $"<i>Show ({objectProps.Count}) filters</i>"
                        ))
                        .TextAnchor(TextAnchor.LowerRight)
                        .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                        .WidthIncRel(1f)
                        .ToButtonGhostly(() => toggle.Value = !toggle.Value),
                        column
                    ),

                    new VerticalContainer(objectProps.Select(objectProp =>
                        new HorizontalContainer([
                            objectProp.Label
                            .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                            .WriteWidthNowTo(ref objectPropLabelWidthMax.Value)
                            .WidthAbsShared(objectPropLabelWidthMax),

                            objectProp.FilterWidget
                            .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                            .WidthIncRel(1f),
                        ], 0f, true)
                        .WidthRel(1f)
                        .HoverBackground(TexUI.HighlightTex)
                    ).ToList<Widget>())
                    .PaddingAbs(Globals.GUI.Pad * 1.5f, 0f, 0f, 0f)
                    .WidthRel(1f)
                    .ToggleDisplay(toggle)
                ])
                .WidthRel(1f)
                .HoverBackground(TexUI.HighlightTex);

                foreach (var objectProp in objectProps)
                {
                    objectProp.FilterWidget.OnChange += HandleFilterChange;
                }
            }

            columnSettingsTabRows.Add(
                row.Background(Verse.Widgets.LightHighlight, columnSettingsTabRows.Count % 2 == 0)
            );
        }

        // Finalize
        Columns = columns;
        SortColumn = sortColumn;
        SortDirection = sorDirection;
        HeaderRows = headerRows;
        PinnedRows = new(10);
        UnfilteredBodyRows = bodyRows;
        FilteredBodyRows = new(bodyRows);
        ColumnsTabWidget = new VerticalContainer(columnSettingsTabRows);
        ActiveFilters = new HashSet<FilterWidget<TObject>>(columnsCount);
        _FilterMode = TableFilterMode.AND;
        ObjectMatchesFilters = ObjectFilterMatchFuncAND;
        ShouldApplyFilters = false;
    }
}
