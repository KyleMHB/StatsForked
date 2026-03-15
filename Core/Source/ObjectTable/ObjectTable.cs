using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using UnityEngine;
using Verse;

namespace Stats;

internal abstract class ObjectTable
{
    internal abstract void Draw(Rect rect);

    internal abstract void ToggleFiltersTab();
}

// Lack of abstraction/leaking abstractions is (almost) intentional here.
// Because abstractions are not free.
internal sealed partial class ObjectTable<TObject> : ObjectTable
{
    //private static readonly TipSignal Manual =
    //    "- Hold (LMB) and move mouse cursor to scroll horizontally.\n" +
    //    "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
    //    "- Hold [Ctrl] and click on a row to pin/unpin it.\n" +
    //    "  - You can pin multiple rows.\n" +
    //    "  - Pinned rows are unaffected by filters.";
    // Filtering
    //public override TableFilterMode FilterMode
    //{
    //    get => field;
    //    set
    //    {
    //        if (value == field) return;

    //        field = value;
    //        MatchRowCells = value switch
    //        {
    //            TableFilterMode.AND => MatchRowCells_AND,
    //            TableFilterMode.OR => MatchRowCells_OR,
    //            _ => throw new NotSupportedException("Unsupported table filtering mode.")
    //        };

    //        OnFilterModeChange?.Invoke(value);
    //        DoFilter = true;
    //    }
    //} = TableFilterMode.AND;
    //public override event Action<TableFilterMode>? OnFilterModeChange;
    //private readonly List<Filter> Filters;
    //private readonly HashSet<Filter> ActiveFilters;
    //private RowCellsMatcher MatchRowCells = MatchRowCells_AND;
    //private static readonly RowCellsMatcher MatchRowCells_AND =
    //(cells, filters) =>
    //{
    //    return filters.All(filter => filter.Widget.Eval(cells[filter.Column]));
    //};
    //private static readonly RowCellsMatcher MatchRowCells_OR =
    //(cells, filters) =>
    //{
    //    return filters.Any(filter => filter.Widget.Eval(cells[filter.Column]));
    //};

    // Sorting
    //private Column SortColumn;
    //private int SortDirection = SortDirectionAscending;
    //private const int SortDirectionAscending = 1;
    //private const int SortDirectionDescending = -1;

    // Columns tab
    //private readonly Widget ColumnsTabWidget;
    //private Vector2 ColumnsTabScrollPosition;

    // Rows
    private readonly List<TObject> _objects;
    private readonly List<int> _rows;
    private int _pinnedRowsCount;
    private int UnpinnedRowsCount => _rows.Count - _pinnedRowsCount;

    // Columns
    private readonly List<Column> _columns;
    private int _pinnedColumnsCount;
    private Column? _currentlyResizedColumn;
    private Column? _currentlyReorderedColumn;

    // Layout
    private float _pinnedRowsHeight;
    private float _unpinnedRowsHeight;
    private float _pinnedColumnsWidth;
    private Vector2 _contentSize;

    // Drawing
    private Vector2 _scrollPosition;
    private Action? _guiAction;

    // Toolbar
    private readonly Toolbar _toolbar;

    public ObjectTable(TableWorker<TObject> tableWorker)
    {
        //tableWorker.OnObjectAdded += AddObject;
        //tableWorker.OnObjectRemoved += RemoveObject;

        // Rows
        List<TObject> objects = tableWorker.InitialObjects;
        List<int> rows = new(objects.Count);
        int objectsCount = objects.Count;
        for (int i = 0; i < objectsCount; i++)
        {
            rows.Add(i);
        }

        // Columns
        List<ColumnDef> columnDefs = tableWorker.TableDef.columns;
        int columnDefsCount = columnDefs.Count;
        List<Column> columns = new(columnDefsCount);
        for (int i = 0; i < columnDefsCount; i++)
        {
            ColumnDef columnDef = columnDefs[i];
            Type workerClass = columnDef.workerClass;
            if (typeof(ColumnWorker<TObject>).IsAssignableFrom(workerClass))
            {
                ColumnWorker<TObject> columnWorker = (ColumnWorker<TObject>)Activator.CreateInstance(workerClass, columnDef);
                Column column = new(columnWorker, tableWorker, this);
                columns.Add(column);
                columnWorker.NotifyRowAdded(objects);
            }
            else
            {
                WarnIncompatibleColumn(columnDef.defName, tableWorker.TableDef.defName);
            }
        }

        // Settings tab
        //List<Widget> columnSettingsTabRows = new(columns.Count);
        //StrongBox<float> columnLabelWidthMax = new(0f);
        //List<Filter> filters = new(columns.Count);

        //Widget MakeColumnTitleRow(Widget columnTitle, Widget filterOrToggle, Column column)
        //{
        //    return new HorizontalContainer([
        //        // TODO: This is a hack to prevent icons from stretching out.
        //        (columnTitle.Get<InlineTexture>() == null
        //            ? columnTitle
        //            : new SingleElementContainer(columnTitle))
        //        .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
        //        .Column(columnLabelWidthMax)
        //        .Tooltip(column.Tooltip),

        //        filterOrToggle,
        //    ], 0f, true)
        //    .WidthRel(1f);
        //}

        //for (int i = 0; i < columns.Count; i++)
        //{
        //    Column column = columns[i];
        //    CellFieldDescriptor[] columnCellFields = column.CellDescriptor.Fields;

        //    if (columnCellFields.Length == 0)
        //        continue;

        //    Widget rowWidget;

        //    if (columnCellFields.Length == 1)
        //    {
        //        CellFieldDescriptor cellField = columnCellFields[0];
        //        rowWidget = MakeColumnTitleRow(
        //            cellField.Label,
        //            cellField.FilterWidget
        //                .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
        //                .WidthIncRel(1f),
        //            column
        //        )
        //            .HoverBackground(TexUI.HighlightTex);

        //        FilterWidget filterWidget = cellField.FilterWidget;
        //        Filter filter = new(column, filterWidget);
        //        filterWidget.OnChange += () => HandleFilterChange(filter);
        //        filters.Add(filter);
        //    }
        //    else
        //    {
        //        StrongBox<float> cellFieldLabelWidthMax = new(0f);
        //        Observable<bool> toggle = new(true);
        //        rowWidget = new VerticalContainer([
        //            MakeColumnTitleRow(
        //                column.Title,
        //                new Label(toggle.Map(state => state
        //                    ? $"<i>Hide ({columnCellFields.Length}) filters</i>"
        //                    : $"<i>Show ({columnCellFields.Length}) filters</i>"
        //                ))
        //                    .TextAnchor(TextAnchor.LowerRight)
        //                    .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
        //                    .WidthIncRel(1f)
        //                    .ToButtonGhostly(() => toggle.Value = !toggle.Value),
        //                column
        //            ),

        //            new VerticalContainer(
        //                columnCellFields.Select(cellField => {
        //                    var row = new HorizontalContainer([
        //                        cellField.Label
        //                            .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
        //                            .Column(cellFieldLabelWidthMax),

        //                        cellField.FilterWidget
        //                            .Ref(out var filterWidget)
        //                            .PaddingAbs(Globals.GUI.Pad, Globals.GUI.PadXs)
        //                            .WidthIncRel(1f),
        //                    ], 0f, true)
        //                        .WidthRel(1f)
        //                        .HoverBackground(TexUI.HighlightTex);

        //                    Filter filter = new(column, filterWidget);
        //                    filters.Add(filter);
        //                    filterWidget.OnChange += () => HandleFilterChange(filter);

        //                    return row;
        //                }).ToList<Widget>()
        //            )
        //                .PaddingAbs(Globals.GUI.Pad * 1.5f, 0f, 0f, 0f)
        //                .WidthRel(1f)
        //                .ToggleDisplay(toggle)
        //        ])
        //            .WidthRel(1f)
        //            .HoverBackground(TexUI.HighlightTex);
        //    }

        //    columnSettingsTabRows.Add(
        //        rowWidget.Background(Verse.Widgets.LightHighlight, columnSettingsTabRows.Count % 2 == 0)
        //    );
        //}

        // Finalize
        _objects = objects;
        _rows = rows;
        _columns = columns;
        if (columns.Count > 0)
        {
            _pinnedColumnsCount = 1;
        }
        _toolbar = new Toolbar(this);
        //SortColumn = columns[0];
        //ColumnsTabWidget = new VerticalContainer(columnSettingsTabRows);
        //Filters = filters;
        //ActiveFilters = new HashSet<Filter>(columns.Count);
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WarnIncompatibleColumn(string columnName, string tableName)
    {
        Log.Warning($"Column \"${columnName}\" is not compatible with table \"${tableName}\", because it does not implement \"${typeof(ColumnWorker<TObject>).Name}\".");
    }
}
