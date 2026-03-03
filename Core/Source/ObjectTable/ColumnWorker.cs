using System.Collections.Generic;
using Stats.ObjectTable.Cells;
using UnityEngine;

namespace Stats.ObjectTable;

public abstract class ColumnWorker
{
    public abstract ColumnDef Def { get; }
}

public abstract class ColumnWorker<TObject> : ColumnWorker
{
    public abstract void DrawCell(Rect rect, Row<TObject> row);

    public abstract float GetCellWidth(Row<TObject> row);

    public abstract void NotifyRowAdded(Row<TObject> row);

    public abstract void NotifyRowRemoved(Row<TObject> row);

    public abstract void RefreshCells();
}

public abstract class ColumnWorker<TObject, TCell> : ColumnWorker<TObject> where TCell : struct, ICell
{
    private readonly List<TCell> _cells = new(250);

    protected abstract TCell MakeCell(TObject @object);

    public override void DrawCell(Rect rect, Row<TObject> row)
    {
        _cells[row.Index].Draw(rect);
    }

    public override float GetCellWidth(Row<TObject> row)
    {
        return _cells[row.Index].Width;
    }

    public override void NotifyRowAdded(Row<TObject> row)
    {
        TCell cell;
        try
        {
            cell = MakeCell(row.Object);
        }
        catch
        {
            cell = default;
        }

        _cells.Add(cell);
    }

    public override void NotifyRowRemoved(Row<TObject> row)
    {
        _cells.ReplaceWithLast(row.Index);
    }

    protected abstract TCell RefreshCell(TCell cell, out bool wasStale);

    public override void RefreshCells()
    {
        List<TCell> cells = _cells;
        int cellsCount = cells.Count;
        for (int i = 0; i < cellsCount; i++)
        {
            TCell originalCell = cells[i];
            TCell possiblyNewCell = RefreshCell(originalCell, out bool wasStale);
            if (wasStale)
            {
                cells[i] = possiblyNewCell;
            }
        }
    }
}

public abstract class StaticColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, ICell
{
    protected override TCell RefreshCell(TCell cell, out bool wasStale)
    {
        wasStale = false;
        return cell;
    }

    public override void RefreshCells() { }
}
