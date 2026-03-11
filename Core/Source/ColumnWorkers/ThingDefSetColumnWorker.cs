using System.Collections.Generic;
using System.Linq;
using Stats.Filters;
using Stats.TableCells;
using Stats.TableWorkers;
using Stats.Widgets;

namespace Stats.ColumnWorkers;

public abstract class ThingDefSetColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IThingDefSetTableCell
{
    protected abstract IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker);

    private static readonly HashSet<Verse.ThingDef> _emptyThingDefHashSet = [];

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                def => def == null ? new() : new(def, def.LabelCap, new ThingDefIcon(def))
            );
        FilterWidget valueFieldFilter = new MTMFilter<Verse.ThingDef?>((int row) => this[row].Value ?? _emptyThingDefHashSet, valueFieldFilterOptions);
        // TODO: Figure out how to efficiently compare cells so that cells with equal values will be grouped together.
        int Compare(int row1, int row2) => row1.CompareTo(row2);
        TableCellFieldDescriptor valueField = new(Def.TitleWidget, valueFieldFilter, Compare);

        return new TableCellDescriptor(TableCellStyleType.String, [valueField]);
    }
}
