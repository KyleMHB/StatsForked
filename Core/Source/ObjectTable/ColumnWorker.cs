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

    public abstract float GetWidth(List<Row<TObject>> rows);

    public abstract void NotifyRowAdded(List<TObject> rows);

    public abstract void NotifyRowAdded(TObject row);

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

    public override float GetWidth(List<Row<TObject>> rows)
    {
        float width = 0f;
        int rowsCount = rows.Count;
        for (int i = 0; i < rowsCount; i++)
        {
            int rowIndex = rows[i].Index;
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
