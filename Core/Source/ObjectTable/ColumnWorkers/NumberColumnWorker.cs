using Stats.ObjectTable.Cells;

namespace Stats.ObjectTable.ColumnWorkers;

public abstract class NumberColumnWorker<T>(ColumnDef ColumnDef) : IColumnWorker<T>
{
    protected virtual string FormatString => "";
    protected abstract CellValueSource<decimal> GetCellValueSource(T @object);
    public Cell GetCell(T @object)
    {
        return new NumberCell(GetCellValueSource(@object), FormatString);
    }
    public CellDescriptor GetCellDescriptor()
    {
        return NumberCell.GetDescriptor(ColumnDef.Title);
    }
}
