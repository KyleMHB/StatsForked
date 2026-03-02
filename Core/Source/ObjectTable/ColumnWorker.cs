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
    protected abstract TCell MakeCell(TObject @object);

    public override void DrawCell(Rect rect, Row<TObject> row)
    {
        throw new System.NotImplementedException();
    }

    public override float GetCellWidth(Row<TObject> row)
    {
        throw new System.NotImplementedException();
    }

    public override void NotifyRowAdded(Row<TObject> row)
    {
        throw new System.NotImplementedException();
    }

    public override void NotifyRowRemoved(Row<TObject> row)
    {
        throw new System.NotImplementedException();
    }

    public override void RefreshCells()
    {
        throw new System.NotImplementedException();
    }
}
