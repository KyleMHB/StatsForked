using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats.ColumnWorkers;

public abstract class ThingDefColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IThingDefCell
{
    public override ColumnType Type => ColumnType.String;
    public override bool ShouldDrawCells => Event.current.IsRepaint() || Event.current.IsLMB();

    protected abstract IEnumerable<Verse.ThingDef?> GetValueFieldFilterOptions(TableWorker tableWorker);

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> valueFieldFilterOptions = GetValueFieldFilterOptions(tableWorker)
            .OrderBy(def => def?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );
        Filter valueFieldFilter = new OTMFilter<Verse.ThingDef?>((int row) => this[row].Value, valueFieldFilterOptions);
        int CompareByCellText(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, CompareByCellText);

        return [valueField];
    }
}
