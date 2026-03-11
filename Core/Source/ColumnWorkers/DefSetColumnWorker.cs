using System.Collections.Generic;
using System.Linq;
using Stats.Filters;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers;

public abstract class DefSetColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IDefSetTableCell
{
    protected abstract IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker);

    private static readonly HashSet<Verse.Def> _emptyDefHashSet = [];

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.Def?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.Def?, NTMFilterOption<Verse.Def?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        FilterWidget valueFieldFilter = new MTMFilter<Verse.Def?>((int row) => this[row].Value ?? _emptyDefHashSet, valueFieldFilterOptions);
        int CompareByCellText(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        TableCellFieldDescriptor valueField = new(Def.TitleWidget, valueFieldFilter, CompareByCellText);

        return new TableCellDescriptor(TableCellStyleType.String, [valueField]);
    }
}
