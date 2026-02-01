namespace Stats.ObjectTable;

public interface IColumnWorker
{
    public CellDescriptor GetCellDescriptor();
}

public interface IColumnWorker<TObject> : IColumnWorker
{
    public Cell GetCell(TObject @object);
}
