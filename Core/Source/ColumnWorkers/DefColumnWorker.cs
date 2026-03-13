using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers;

public abstract class DefColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IDefTableCell
{
    public override ColumnType Type => ColumnType.String;

    protected abstract IEnumerable<Verse.Def?> GetValueFieldFilterOptions(TableWorker tableWorker);

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.Def?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.Def?, NTMFilterOption<Verse.Def?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        FilterWidget valueFieldFilter = new OTMFilter<Verse.Def?>((int row) => this[row].Value, valueFieldFilterOptions);
        int CompareByDefLabel(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, CompareByDefLabel);

        return [valueField];
    }
}
