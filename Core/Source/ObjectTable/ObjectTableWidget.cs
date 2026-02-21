using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Stats.ObjectTable.Cells;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable;

public abstract class ObjectTableWidget
{
    public const float CellPadHor = 12f;
    public const float CellPadVer = 4f;
    //public abstract TableFilterMode FilterMode { get; set; }
    //public abstract event Action<TableFilterMode> OnFilterModeChange;
    public abstract void Draw(Rect rect, bool showSettingsMenu);
    //public abstract void ResetFilters();
    //public abstract void ToggleFilterMode();

    //public enum TableFilterMode
    //{
    //    AND = 0,
    //    OR = 1,
    //}
}

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
    private readonly List<Column> _columns;
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
    private const int InitialRowCapacity = 250;
    //private readonly List<Row> HeaderRows;
    private readonly float _headerRowHeight;
    private readonly List<Row> _pinnedRows = new(10);
    private float _pinnedRowsHeight;
    private readonly List<Row> _unpinnedRows;
    private readonly List<Row> _filteredUnpinnedRows;
    private float _filteredUnpinnedRowsHeight;
    private Row? _rowToPinOrUnpin;
    private Vector2 _scrollPosition;
    private static readonly Color ColumnSeparatorLineColor = new(1f, 1f, 1f, 0.05f);
    private static readonly Color PinnedRowsBGColor = Verse.Widgets.HighlightStrongBgColor.ToTransparent(0.1f);
    //private Vector2 ColumnsTabScrollPosition;

    public ObjectTableWidget(TableWorker<TObject> tableWorker)
    {
        //tableWorker.OnObjectAdded += AddObject;
        //tableWorker.OnObjectRemoved += RemoveObject;

        // Columns
        List<Column> columns = new(tableWorker.TableDef.columns.Count);

        for (int i = 0; i < tableWorker.TableDef.columns.Count; i++)
        {
            ColumnDef columnDef = tableWorker.TableDef.columns[i];
            if (columnDef.Worker is IColumnWorker<TObject> columnWorker)
            {
                int cellIndex = columns.Count;
                Column column = new(cellIndex, columnDef, columnWorker, tableWorker, cellIndex == 0);

                columns.Add(column);
            }
            else
            {
                Log.Warning($"Column \"${columnDef.defName}\" is not compatible with table \"${tableWorker.TableDef.defName}\", because it does not implement \"${typeof(IColumnWorker<TObject>).Name}\".");
            }
        }

        // Rows
        List<Row> rows = new(InitialRowCapacity);

        foreach (TObject @object in tableWorker.InitialObjects)
        {
            Row row = new(columns, @object, this);

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
        //SortColumn = columns[0];
        //HeaderRows = [new ColumnTitlesRow(columns)];
        _unpinnedRows = rows;
        _filteredUnpinnedRows = [.. rows];
        //ColumnsTabWidget = new VerticalContainer(columnSettingsTabRows);
        //Filters = filters;
        //ActiveFilters = new HashSet<Filter>(columns.Count);
    }
}
