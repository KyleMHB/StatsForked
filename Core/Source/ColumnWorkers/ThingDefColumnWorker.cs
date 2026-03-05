using System.Collections.Generic;
using System.Linq;
using Stats.FilterWidgets;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers;

public abstract class ThingDefColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IThingDefTableCell
{
    protected abstract IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker);

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        FilterWidget valueFieldFilter = new OTMFilter<Verse.ThingDef?>((int row) => this[row].Value, valueFieldFilterOptions);
        int CompareByCellText(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        TableCellFieldDescriptor valueField = new(Def.Title, valueFieldFilter, CompareByCellText);

        return new TableCellDescriptor(TableCellStyleType.String, [valueField]);
    }
}
