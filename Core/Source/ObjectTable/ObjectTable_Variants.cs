using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private void ToggleVariants()
    {
        if (SupportsVariants == false)
        {
            return;
        }

        SetVariantsMode(_showVariants == false);
    }

    private void SetVariantsMode(bool showVariants)
    {
        if (SupportsVariants == false || _showVariants == showVariants)
        {
            return;
        }

        _showVariants = showVariants;
        string? sortColumnDefName = _sortColumn?.Def.defName;
        int sortDirection = _sortDirection;
        List<string> visibleColumnDefNames = CaptureVisibleColumnDefNames();
        List<FilterPresetState> filterStates = CaptureFilterPresetStates();

        RebuildRowsAndColumns(GetCurrentObjects(), visibleColumnDefNames);
        ApplyFilterPresetStates(filterStates);

        if (sortColumnDefName?.Length > 0)
        {
            _sortColumn = _columns.FirstOrDefault(column => column.Def.defName == sortColumnDefName) ?? _sortColumn;
            _sortDirection = sortDirection;
            SortRows();
            ApplyFilters();
        }
    }

    private void RebuildRowsAndColumns(List<TObject> objects, List<string> visibleColumnDefNames)
    {
        ResetRows(objects);
        ResetColumns(visibleColumnDefNames);
        SortRows();
        ApplyFilters();
    }

    private void ResetRows(List<TObject> objects)
    {
        _objects.Clear();
        _objects.AddRange(objects);

        _rowOrder.Clear();
        _rows.Clear();
        int objectsCount = _objects.Count;
        for (int i = 0; i < objectsCount; i++)
        {
            _rowOrder.Add(i);
            _rows.Add(i);
        }

        _topRowsCount = 0;
    }

    private void ResetColumns(List<string> visibleColumnDefNames)
    {
        foreach (Column column in _columns)
        {
            UnregisterColumnFilters(column);
            _toolbar.NotifyColumnRemoved(column);
        }

        _columns.Clear();
        _leftColumnsCount = 0;
        _sortColumn = null;
        _reorderedColumn = null;
        _pressedColumn = null;

        IEnumerable<ColumnDef> columnDefs = visibleColumnDefNames
            .Select(defName => _tableWorker.Def.columns.FirstOrDefault(column => column.defName == defName))
            .Where(column => column != null)!;

        foreach (ColumnDef columnDef in columnDefs)
        {
            TryAddColumn(columnDef, notifyToolbar: true, applyFilters: false);
        }

        if (_columns.Count > 0)
        {
            _leftColumnsCount = 1;
            _sortColumn = _columns[0];
        }
    }
}
