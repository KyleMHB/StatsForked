using Stats.ObjectTable.Cells;

namespace Stats.ObjectTable.ColumnWorkers;

public abstract class BooleanColumnWorker<T>(ColumnDef ColumnDef) : IColumnWorker<T>
{
    protected abstract CellValueSource<bool> GetCellValueSource(T @object);
    public Cell GetCell(T @object)
    {
        return new BooleanCell(GetCellValueSource(@object));
    }
    public CellDescriptor GetCellDescriptor()
    {
        return BooleanCell.GetDescriptor(ColumnDef.Title);
    }
}
