using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.ObjectTable.ColumnWorkers;
using Stats.ObjectTable.TableWorkers;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

public abstract class ObjectTableWidget
{
    public const float CellPadHor = 12f;
    public const float CellPadVer = 4f;
    public abstract TableFilterMode FilterMode { get; set; }
    public abstract event Action<TableFilterMode> OnFilterModeChange;
    public abstract void Draw(Rect rect, bool showSettingsMenu);
    public abstract void ResetFilters();
    public abstract void ToggleFilterMode();

    public enum TableFilterMode
    {
        AND = 0,
        OR = 1,
    }
}

internal sealed partial class ObjectTableWidget<TObject> : ObjectTableWidget
{
    private Column SortColumn;
    private int SortDirection = SortDirectionAscending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;
    private static readonly Color SortIndicatorColor = Color.yellow.ToTransparent(0.3f);
    private const float SortIndicatorHeight = 5f;
    private readonly List<Column> Columns;
    private readonly List<Column> ColumnsVisible;
    private readonly List<Column> ColumnsVisiblePinned;
    private readonly List<Column> ColumnsVisibleUnpinned;
    private readonly Widget ColumnsTabWidget;
    private readonly List<Filter> Filters;
    private readonly HashSet<Filter> ActiveFilters;
    public override TableFilterMode FilterMode
    {
        get => field;
        set
        {
            if (value == field) return;

            field = value;
            MatchRowCells = value switch
            {
                TableFilterMode.AND => MatchRowCells_AND,
                TableFilterMode.OR => MatchRowCells_OR,
                _ => throw new NotSupportedException("Unsupported table filtering mode.")
            };

            OnFilterModeChange?.Invoke(value);
            DoFilter = true;
        }
    } = TableFilterMode.AND;
    public override event Action<TableFilterMode>? OnFilterModeChange;
    private RowCellsMatcher MatchRowCells = MatchRowCells_AND;
    private static readonly RowCellsMatcher MatchRowCells_AND =
    (cells, filters) =>
    {
        return filters.All(filter => filter.Widget.Eval(cells[filter.Column]));
    };
    private static readonly RowCellsMatcher MatchRowCells_OR =
    (cells, filters) =>
    {
        return filters.Any(filter => filter.Widget.Eval(cells[filter.Column]));
    };
    private const int InitialRowCapacity = 250;
    private readonly List<Row> HeaderRows;
    private float HeaderRowsHeight;
    private readonly List<ObjectRow> PinnedRows = new(10);
    private float PinnedRowsHeight;
    private readonly List<ObjectRow> UnpinnedRows;
    private float UnpinnedRowsHeight;
    private Vector2 ScrollPosition;
    private static readonly Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private static readonly Color PinnedRowsBGColor = Verse.Widgets.HighlightStrongBgColor.ToTransparent(0.1f);
    private Vector2 ColumnsTabScrollPosition;
    private bool DoFilter;
    private bool DoSort = true;
    private bool DoResize = true;
    private bool DoUpdateCachedColumns = true;
    private readonly Stack<Column> ColumnsToRefresh;
    public ObjectTableWidget(TableWorker<TObject> worker)
    {
        if (worker is TableWorker<TObject>.IStreaming streamingWorker)
        {
            streamingWorker.OnObjectAdded += AddObject;
            streamingWorker.OnObjectRemoved += RemoveObject;
        }

        var columns = new List<Column>();

        foreach (var columnDef in worker.TableDef.columns)
        {
            if (columnDef.Worker is IColumnWorker<TObject> columnWorker)
            {
                columns.Add(new(columnDef, columnWorker, this));
            }
            else
            {
                Log.Warning($"Column \"${columnDef.defName}\" is not compatible with table \"${worker.TableDef.defName}\".");
            }
        }

        columns[0].IsPinned = true;

        // Rows
        var rows = new List<ObjectRow>(InitialRowCapacity);

        foreach (var @object in worker.InitialObjects)
        {
            var row = new ObjectRow(columns, @object);

            rows.Add(row);
        }

        // Settings tab
        var columnSettingsTabRows = new List<Widget>(columns.Count);
        var columnLabelWidthMax = new StrongBox<float>(0f);
        var filters = new List<Filter>(columns.Count);

        Widget MakeColumnTitleRow(Widget columnTitle, Widget filterOrToggle, Column column)
        {
            return new HorizontalContainer([
                // TODO: This is a hack to prevent icons from stretching out.
                (columnTitle.Get<InlineTexture>() == null
                    ? columnTitle
                    : new SingleElementContainer(columnTitle))
                .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                .Column(columnLabelWidthMax)
                .Foreground(rect =>
                {
                    if (Event.current.type != EventType.Repaint || column.IsVisible) return;

                    var origGUIColor = GUI.color;
                    GUI.color = Globals.GUI.TextColorSecondary;

                    Verse.Widgets.DrawLineHorizontal(rect.x, rect.y + rect.height / 2f, rect.width);

                    GUI.color = origGUIColor;
                })
                .ToButtonGhostly(column.ToggleVisibility)
                .Tooltip(column.Tooltip),

                filterOrToggle,
            ], 0f, true)
            .WidthRel(1f);
        }

        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var objectProps = column.GetParts().ToList();

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
                .HoverBackground(TexUI.HighlightTex);

                var filter = new Filter(column, objectProp.FilterWidget);
                filters.Add(filter);
                objectProp.FilterWidget.OnChange += () => HandleFilterChange(filter);
            }
            else
            {
                var objectPropLabelWidthMax = new StrongBox<float>(0f);
                var toggle = new Observable<bool>(true);
                row = new VerticalContainer([
                    MakeColumnTitleRow(
                        column.Title,
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

                    new VerticalContainer(
                        objectProps.Select(objectProp => {
                            var row = new HorizontalContainer([
                                objectProp.Label
                                .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                                .Column(objectPropLabelWidthMax),

                                objectProp.FilterWidget
                                .Ref(out var filterWidget)
                                .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
                                .WidthIncRel(1f),
                            ], 0f, true)
                            .WidthRel(1f)
                            .HoverBackground(TexUI.HighlightTex);

                            var filter = new Filter(column, filterWidget);
                            filters.Add(filter);
                            filterWidget.OnChange += () => HandleFilterChange(filter);

                            return row;
                        }).ToList<Widget>()
                    )
                    .PaddingAbs(Globals.GUI.Pad * 1.5f, 0f, 0f, 0f)
                    .WidthRel(1f)
                    .ToggleDisplay(toggle)
                ])
                .WidthRel(1f)
                .HoverBackground(TexUI.HighlightTex);
            }

            columnSettingsTabRows.Add(
                row.Background(Verse.Widgets.LightHighlight, columnSettingsTabRows.Count % 2 == 0)
            );
        }

        // Finalize
        Columns = columns;
        ColumnsVisible = new(columns.Count);
        ColumnsVisiblePinned = new(columns.Count);
        ColumnsVisibleUnpinned = new(columns.Count);
        ColumnsToRefresh = new(columns.Count);
        SortColumn = columns[0];
        HeaderRows = [new ColumnTitlesRow(columns)];
        UnpinnedRows = rows;
        ColumnsTabWidget = new VerticalContainer(columnSettingsTabRows);
        Filters = filters;
        ActiveFilters = new HashSet<Filter>(columns.Count);
    }
    // Note: Add/Remove methods have to be as fast as possible
    // because they can be called multiple times in a row.
    //
    // TODO: Do not add/remove rows if the table is not shown on the screen.
    private void AddObject(TObject @object)
    {
        UnpinnedRows.Add(new ObjectRow(Columns, @object));
        //DoSort = true;
        SortRows(UnpinnedRows);
        if (ActiveFilters.Count > 0)
        {
            DoFilter = true;
        }
        DoResize = true;
    }
    private void RemoveObject(TObject @object)
    {
        PinnedRows.RemoveWhere(row => DisposeOfRowIfMatch(row, @object));
        UnpinnedRows.RemoveWhere(row => DisposeOfRowIfMatch(row, @object));

        DoResize = true;
    }
    private static bool DisposeOfRowIfMatch(ObjectRow row, TObject @object)
    {
        var isObj = row.Object.Equals(@object);

        if (isObj)
        {
            row.Dispose();
        }

        return isObj;
    }
}
