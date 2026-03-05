using Stats.FilterWidgets;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers;

public abstract class NumberColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, INumberTableCell
{
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        FilterWidget valueFieldFilter = new NumberFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        TableCellFieldDescriptor valueField = new(Def.Title, valueFieldFilter, Compare);

        return new TableCellDescriptor(TableCellStyleType.Number, [valueField]);
    }
}
