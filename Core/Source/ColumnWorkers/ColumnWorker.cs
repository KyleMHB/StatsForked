using System.Collections.Generic;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers;

public abstract class ColumnWorker
{
    public abstract ColumnDef Def { get; }
}

public abstract class ColumnWorker<TObject> : ColumnWorker
{
    public abstract void DrawCell(Rect rect, int row);

    public abstract float GetWidth(List<int> rows);

    public abstract void NotifyRowAdded(List<TObject> rows);

    public abstract void NotifyRowAdded(TObject row);

    public abstract void NotifyRowRemoved(int row);

    public abstract void RefreshCells();

    public abstract TableCellDescriptor GetCellDescriptor(TableWorker tableWorker);
}

public abstract class ColumnWorker<TObject, TCell> : ColumnWorker<TObject> where TCell : struct, ITableCell
{
    private readonly List<TCell> _cells = new(250);

    protected TCell this[int index] => _cells[index];

    protected abstract TCell MakeCell(TObject @object);

    public override void DrawCell(Rect rect, int row)
    {
        _cells[row].Draw(rect);
    }

    public override float GetWidth(List<int> rows)
    {
        float width = 0f;
        int rowsCount = rows.Count;
        for (int i = 0; i < rowsCount; i++)
        {
            int rowIndex = rows[i];
            float cellWidth = _cells[rowIndex].Width;
            if (width < cellWidth)
            {
                width = cellWidth;
            }
        }

        return width;
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
        TCell cell;
        try
        {
            cell = MakeCell(row);
        }
        catch
        {
            cell = default;
        }

        _cells.Add(cell);
    }

    public override void NotifyRowRemoved(int row)
    {
        _cells.ReplaceWithLast(row);
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

public abstract class StaticColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, ITableCell
{
    protected override TCell RefreshCell(TCell cell, out bool wasStale)
    {
        wasStale = false;
        return cell;
    }

    public override void RefreshCells() { }
}
