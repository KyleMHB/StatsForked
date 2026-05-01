using System.Collections.Generic;
using Stats.ColumnWorkers.Cells;
using System;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers;

public abstract class BooleanColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IBooleanCell
{
    public override ColumnType Type => ColumnType.Boolean;

    public override float GetWidth(List<int> rows)
    {
        return Verse.Text.LineHeight;
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter valueFieldFilter = new BooleanFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2]);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, Compare);

        return [valueField];
    }
}

public abstract class BooleanColumnWorker<TObject> : ColumnWorker<TObject>
{
    public override ColumnType Type => ColumnType.Boolean;
    public override bool IsRefreshable => false;

    private readonly List<bool> _values = new(250);
    private readonly HashSet<string> _getValueExceptionKeys = [];

    protected abstract bool GetValue(TObject @object);

    public override void DrawCell(Rect rect, int row)
    {
        BooleanCell.Draw(rect, _values[row]);
    }

    public override float GetWidth(List<int> rows)
    {
        return Verse.Text.LineHeight;
    }

    public override void NotifyRowAdded(List<TObject> rows)
    {
        int rowsCount = rows.Count;
        for (int i = 0; i < rowsCount; i++)
        {
            NotifyRowAdded(rows[i]);
        }
    }

    public override void NotifyRowAdded(TObject row)
    {
        bool value;
        try
        {
            value = GetValue(row);
        }
        catch (Exception exception)
        {
            LogGetValueException(row, exception);
            value = default;
        }

        _values.Add(value);
    }

    public override void NotifyRowRemoved(int row)
    {
        _values.ReplaceWithLast(row);
    }

    public override bool RefreshCells()
    {
        return false;
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter valueFieldFilter = new BooleanFilter((int row) => _values[row]);
        int Compare(int row1, int row2) => _values[row1].CompareTo(_values[row2]);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, Compare);

        return [valueField];
    }

    private void LogGetValueException(TObject row, Exception exception)
    {
        string key = $"{GetType().FullName}:{exception.GetType().FullName}:{exception.Message}";
        if (_getValueExceptionKeys.Add(key) == false)
        {
            return;
        }

        string rowText;
        try
        {
            rowText = row?.ToString() ?? "<null>";
        }
        catch
        {
            rowText = "<unprintable>";
        }

        Log.Error($"Stats failed to create boolean cell for worker {GetType().FullName}, column \"{Def.defName}\", row {rowText}: {exception}");
    }
}
