using Stats.ObjectTable.Cells;

namespace Stats.ObjectTable;

public interface IColumnWorker
{
    public CellDescriptor GetCellDescriptor(TableWorker tableWorker);
}

public interface IColumnWorker<TObject> : IColumnWorker
{
    public Cell GetCell(TObject @object);
}
