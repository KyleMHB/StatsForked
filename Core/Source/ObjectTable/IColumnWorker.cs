using Stats.ObjectTable.Cells;

namespace Stats.ObjectTable;

public interface IColumnWorker
{
    public ColumnDef Def { get; }
}

public interface IColumnWorker<TObject> : IColumnWorker
{
    public Cell MakeCell(TObject @object);
    public CellDescriptor GetCellDescriptor(TableWorker tableWorker);
}
