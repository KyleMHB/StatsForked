using Stats.ObjectTable.Cells;

namespace Stats.ObjectTable.ColumnWorkers;

public interface IColumnWorker<TObject>
{
    public Cell GetCell(TObject @object);
    public CellDescriptor GetCellDescriptor();
}
