namespace Stats;

public abstract class ColumnWorker : IColumnWorker
{
    // Why is this not in ColumnDef?
    //
    // This part of a column representation is governed by column worker,
    // because only column worker knows what type of data it encapsulates.
    public IColumnWorker.CellStyleType CellStyle { get; }
    public ColumnDef Def { get; }
    protected ColumnWorker(ColumnDef columnDef, IColumnWorker.CellStyleType cellStyle)
    {
        Def = columnDef;
        CellStyle = cellStyle;
    }
}
