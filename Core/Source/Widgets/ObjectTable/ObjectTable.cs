using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

public abstract class ObjectTable
{
    public const float CellPadHor = 12f;
    public const float CellPadVer = 4f;
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

public sealed partial class ObjectTable<TObject> : ObjectTable
{
    private readonly List<ColumnWorker<TObject>> Columns;
    private readonly List<ColumnWorker<TObject>> ColumnsVisible;
    private readonly List<ColumnWorker<TObject>> ColumnsVisiblePinned;
    private readonly List<ColumnWorker<TObject>> ColumnsVisibleUnpinned;
    private readonly Widget ColumnsTabWidget;
    internal ColumnWorker<TObject> SortColumn;
    internal int SortDirection = SortDirectionAscending;
    internal const int SortDirectionAscending = 1;
    internal const int SortDirectionDescending = -1;
    private readonly List<FilterWidget<TObject>> Filters;
    private readonly HashSet<FilterWidget<TObject>> ActiveFilters;
    public override TableFilterMode FilterMode
    {
        get => field;
        set
        {
            if (value == field) return;

            field = value;
            ObjectMatchesFilters = value switch
            {
                TableFilterMode.AND => ObjectFilterMatchFuncAND,
                TableFilterMode.OR => ObjectFilterMatchFuncOR,
                _ => throw new NotSupportedException("Unsupported table filtering mode.")
            };

            OnFilterModeChange?.Invoke(value);
            DoFilter = true;
        }
    } = TableFilterMode.AND;
    public override event Action<TableFilterMode>? OnFilterModeChange;
    private ObjectFilterMatchFunc ObjectMatchesFilters = ObjectFilterMatchFuncAND;
    private static readonly ObjectFilterMatchFunc ObjectFilterMatchFuncAND =
    (@object, filters) =>
    {
        return filters.All(filter => filter.Eval(@object));
    };
    private static readonly ObjectFilterMatchFunc ObjectFilterMatchFuncOR =
    (@object, filters) =>
    {
        return filters.Any(filter => filter.Eval(@object));
    };
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
    private bool DoRefreshColumns;
    public ObjectTable(List<ColumnWorker<TObject>> columns, IEnumerable<TObject> initialObjects)
    {
        columns[0].IsPinned = true;

        // TODO: Maybe the constructor should accept list from the beginning.
        var initialObjectsArray = initialObjects.ToArray();

        // Rows
        var rows = new List<ObjectRow>(initialObjectsArray.Length);

        foreach (var @object in initialObjectsArray)
        {
            var row = new ObjectRow(columns, @object);

            rows.Add(row);
        }

        // Settings tab
        var columnSettingsTabRows = new List<Widget>(columns.Count);
        var columnLabelWidthMax = new StrongBox<float>(0f);
        var filters = new List<FilterWidget<TObject>>(columns.Count);

        Widget MakeColumnTitleRow(Widget columnTitle, Widget filterOrToggle, ColumnWorker<TObject> column)
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
                .ToButtonGhostly(column.Toggle)
                .Tooltip(column.Tooltip),

                filterOrToggle,
            ], 0f, true)
            .WidthRel(1f);
        }

        for (int i = 0; i < columns.Count; i++)
        {
            var column = columns[i];
            var objectProps = column.GetObjectProps(initialObjectsArray).ToList();

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

                filters.Add(objectProp.FilterWidget);
                objectProp.FilterWidget.OnChange += HandleFilterChange;
            }
            else
            {
                var objectPropLabelWidthMax = new StrongBox<float>(0f);
                var toggle = new Observable<bool>(true);
                row = new VerticalContainer([
                    MakeColumnTitleRow(
                        column.ColumnDef.Title,
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

                            filters.Add(filterWidget);
                            filterWidget.OnChange += HandleFilterChange;

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
        ColumnsVisible = new(columns.Capacity);
        ColumnsVisiblePinned = new(columns.Capacity);
        ColumnsVisibleUnpinned = new(columns.Capacity);
        SortColumn = columns[0];
        HeaderRows = [new ColumnTitlesRow(columns, this)];
        UnpinnedRows = rows;
        ColumnsTabWidget = new VerticalContainer(columnSettingsTabRows);
        Filters = filters;
        ActiveFilters = new HashSet<FilterWidget<TObject>>(columns.Count);
    }
    // Note: Add/Remove methods have to be as fast as possible
    // because they can be called multiple times in a row.
    public void AddObject(TObject @object)
    {
    }
    public void RemoveObject(TObject @object)
    {
    }
}
