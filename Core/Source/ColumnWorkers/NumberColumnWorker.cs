using System.Collections.Generic;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers;

public abstract class NumberColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, INumberTableCell
{
    public override ColumnType Type => ColumnType.Number;

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        FilterWidget valueFieldFilter = new NumberFilter((int row) => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, Compare);

        return [valueField];
    }
}
