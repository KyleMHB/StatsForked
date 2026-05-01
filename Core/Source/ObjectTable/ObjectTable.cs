using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats;

internal abstract class ObjectTable
{
    internal abstract void Draw(Rect rect);

    internal abstract void NotifyParentWindowClosed();
}

// Lack of abstraction/leaking abstractions is (almost) intentional here.
// Because abstractions are not free.
internal sealed partial class ObjectTable<TObject> : ObjectTable
{
    private static readonly TipSignal _manual =
        "- Hold (LMB) and move mouse cursor to scroll horizontally.\n" +
        "- Hold [Ctrl] and click on a column's name to pin/unpin it.\n" +
        "- Hold [Ctrl] and click on a row to pin/unpin it.\n" +
        "  - You can pin multiple rows.\n" +
        "  - Pinned rows are unaffected by filters.\n" +
        "- Pull top part of the window to change height.\n" +
        "- Double click to reset window height.";
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
    private Column? _sortColumn;
    private int _sortDirection = SortDirectionAscending;
    private const int SortDirectionAscending = 1;
    private const int SortDirectionDescending = -1;

    // Filters tab

    // Rows
    private readonly List<TObject> _objects;
    private readonly List<int> _rowOrder;
    private readonly List<int> _rows;
    private int _topRowsCount;
    private int BottomRowsCount => _rows.Count - _topRowsCount;

    // Columns
    private readonly List<Column> _columns;
    private int _leftColumnsCount;
    private int RightColumnsCount => _columns.Count - _leftColumnsCount;
    private ReadOnlyListSegment<Column> LeftColumns => new(_columns, 0, _leftColumnsCount);
    private ReadOnlyListSegment<Column> RightColumns => new(_columns, _leftColumnsCount, RightColumnsCount);
    private Column? _reorderedColumn;
    private Column? _pressedColumn;

    // Layout
    private float _topRowsHeight;
    private float _bottomRowsHeight;
    private float _leftColumnsWidth;
    private Vector2 _contentSize;

    // Drawing
    private Vector2 _scrollPosition;
    // A way to defer any code that would otherwise modify
    // the collection that is currently being iterated over.
    // Primarily GUI event handlers.
    private Action? _beforeDraw;
    private bool _rightPartIsPanned;

    // Toolbar
    private readonly Toolbar _toolbar;

    // Misc
    private readonly TableWorker<TObject> _tableWorker;
    private readonly IVariantTableWorker<TObject>? _variantTableWorker;
    private readonly HashSet<string> _missingColumnWarnings = [];
    private bool _showVariants;
    internal bool SupportsVariants => _variantTableWorker?.SupportsVariants == true;
    internal bool ShowVariants => _showVariants;
    private QualityCategory _quality = QualityCategory.Normal;
    internal bool SupportsQuality => typeof(TObject) == typeof(DefBasedObject)
        && _tableWorker.CompatibleColumns.Any(column => column is StatColumnDef);
    internal QualityCategory Quality => _quality;

    public ObjectTable(TableWorker<TObject> tableWorker)
    {
        //tableWorker.OnObjectAdded += AddObject;
        //tableWorker.OnObjectRemoved += RemoveObject;

        // Finalize
        _tableWorker = tableWorker;
        _variantTableWorker = tableWorker as IVariantTableWorker<TObject>;
        _showVariants = _variantTableWorker?.ShowVariantsByDefault == true;
        _objects = [];
        _rowOrder = [];
        _rows = [];
        _columns = [];
        _toolbar = new Toolbar(this);
        RegisterTableFilters();
        RebuildRowsAndColumns(GetCurrentObjects(), tableWorker.Def.columns.Select(column => column.defName).ToList());
    }

    [MethodImpl(MethodImplOptions.NoInlining)]
    private static void WarnIncompatibleColumn(string columnName, string tableName)
    {
        Log.Warning($"Column \"${columnName}\" is not compatible with table \"${tableName}\", because it does not implement \"${typeof(ColumnWorker<TObject>).Name}\".");
    }

    internal override void NotifyParentWindowClosed()
    {
        _rightPartIsPanned = false;
        _reorderedColumn = null;
        _pressedColumn = null;
        _filtersWindow?.Close(false);
        for (int i = 0; i < _columns.Count; i++)
        {
            Column column = _columns[i];
            column.NotifyParentWindowClosed();
        }
    }

    private List<TObject> GetCurrentObjects()
    {
        List<TObject> objects = _variantTableWorker?.GetObjects(_showVariants) ?? _tableWorker.InitialObjects;
        return ApplyQuality(objects);
    }

    private List<TObject> ApplyQuality(List<TObject> objects)
    {
        if (SupportsQuality == false)
        {
            return objects;
        }

        return objects
            .Cast<DefBasedObject>()
            .Select(@object => @object.WithQuality(_quality))
            .Cast<TObject>()
            .ToList();
    }
}
