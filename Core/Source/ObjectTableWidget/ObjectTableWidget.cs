using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.FilterWidgets;
using Stats.TableWorkers;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public abstract class ObjectTableWidget
{
    internal const float CellPadHor = 12f;
    internal const float CellPadVer = 4f;

    public static readonly float CellContentSpacing = Globals.GUI.PadSm;
    //public abstract TableFilterMode FilterMode { get; set; }

    //public abstract event Action<TableFilterMode> OnFilterModeChange;

    internal abstract void Draw(Rect rect, bool showSettingsMenu);

    //public abstract void ResetFilters();

    //public abstract void ToggleFilterMode();

    //public enum TableFilterMode
    //{
    //    AND = 0,
    //    OR = 1,
    //}
}

// Lack of abstraction/leaking abstractions is (almost) intentional here.
// Because abstractions are not free.
internal sealed partial class ObjectTableWidget<TObject> : ObjectTableWidget
{
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

    //private Column SortColumn;
    //private int SortDirection = SortDirectionAscending;
    //private const int SortDirectionAscending = 1;
    //private const int SortDirectionDescending = -1;
    //private readonly Widget ColumnsTabWidget;
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
    //private Vector2 ColumnsTabScrollPosition;
    private readonly List<Column> _columns;
    private ReadOnlyListSegment<Column> _PinndeColumns => new(_columns, 0, _pinnedColumnsCount);
    private int _pinnedColumnsCount;
    private float _pinnedColumnsWidth;
    private ReadOnlyListSegment<Column> _UnpinndeColumns => new(_columns, _pinnedColumnsCount, _columns.Count - _pinnedColumnsCount);
    private float _unpinnedColumnsWidth;
    private float _headerRowHeight;
    private const int _InitialRowCapacity = 250;
    // Here's the idea:
    // - Iterate through List<TObject> filtering each row.
    // - Put filtered rows as Row<TObject> in a list.
    // - Sort the list. Use row's index for stable sort (when two rows have the same cell values)?
    // - Pass Row<TObject>s from the list to column workers for drawing/resizing/etc.
    //
    // This will make it possible to have column workers with List-based cells cache.
    // Which in turn will give us O(1) on accessing column's cells.
    //
    // Objects from List<TObject> are removed by replacing removed object with the last one.
    // Although we still need to find it, so it'll be O(n).
    // We clear the working set (List<Row<TObject>>) and rebuild it on next GUI event, so we don't have
    // to remove Row<TObject> from it at the same time, which will be O(n)
    // for every removed object (and the game can despawn many objects at once).
    private readonly List<TObject> _objects;
    private readonly List<TableRow<TObject>> _filteredRows;
    private ReadOnlyListSegment<TableRow<TObject>> _PinndeRows => new(_filteredRows, 0, _pinnedRowsCount);
    private int _pinnedRowsCount;
    private float _pinnedRowsHeight;
    private ReadOnlyListSegment<TableRow<TObject>> _UnpinndeRows => new(_filteredRows, _pinnedRowsCount, _filteredRows.Count - _pinnedRowsCount);
    private float _unpinnedRowsHeight;
    private static readonly Color _columnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private static readonly Color _pinnedRowsBGColor = Verse.Widgets.HighlightStrongBgColor.ToTransparent(0.1f);
    private Vector2 _scrollPosition;
    private Action? _guiAction;
    private Vector2 _contentSize;

    public ObjectTableWidget(TableWorker<TObject> tableWorker)
    {
        //tableWorker.OnObjectAdded += AddObject;
        //tableWorker.OnObjectRemoved += RemoveObject;

        // Columns
        List<ColumnDef> columnDefs = tableWorker.TableDef.columns;
        int columnDefsCount = columnDefs.Count;
        List<Column> columns = new(columnDefsCount);
        for (int i = 0; i < columnDefsCount; i++)
        {
            ColumnDef columnDef = columnDefs[i];
            Type workerClass = columnDef.workerClass;
            if (workerClass.IsAssignableFrom(typeof(ColumnWorker<TObject>)))
            {
                ColumnWorker<TObject> columnWorker = (ColumnWorker<TObject>)Activator.CreateInstance(workerClass, columnDef);
                int cellIndex = columns.Count;
                Column column = new(cellIndex, columnWorker, tableWorker, this);
                columns.Add(column);
            }
            else
            {
                WarnIncompatibleColumn(columnDef.defName, tableWorker.TableDef.defName);
            }
        }

        // Rows
        List<TableRow<TObject>> rows = new(_InitialRowCapacity);
        foreach (TObject @object in tableWorker.InitialObjects)
        {
            TableRow<TObject> row = new(rows.Count, @object);
            rows.Add(row);
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
        _columns = columns;
        if (columns.Count > 0)
        {
            _pinnedColumnsCount = 1;
        }
        _filteredRows = rows;
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

    private readonly struct ReadOnlyListSegment<T>
    {
        public readonly int Length;

        private readonly List<T> _list;
        private readonly int _start;

        public ReadOnlyListSegment(List<T> list, int start, int length)
        {
            _list = list;
            _start = start;
            Length = length;
        }

        public ReadOnlyListSegment(List<T> list) : this(list, 0, list.Count) { }

        public T this[int index]
        {
            get
            {
                if ((uint)index >= (uint)Length)
                {
                    throw new ArgumentOutOfRangeException("index");
                }

                return _list[index + _start];
            }
        }

        public ReadOnlyListSegment<T> Slice(int start, int length)
        {
            return new ReadOnlyListSegment<T>(_list, _start + start, length);
        }
    }
}
