using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;

namespace Stats.ColumnWorkers;

public abstract class ThingDefCountColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IThingDefCountTableCell
{
    public override ColumnType Type => ColumnType.Number;
    public override bool ShouldDrawCells => Event.current.IsRepaint() || Event.current.IsLMB();

    protected abstract IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker);

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Widget countFieldLabel = new Label("Amount");
        Filter countFilter = new NumberFilter((int row) => this[row].Count);
        int CompareByCount(int row1, int row2) => this[row1].Count.CompareTo(this[row2].Count);
        CellField countField = new(countFieldLabel, countFilter, CompareByCount);

        Widget thingDefFieldLabel = new Label("Type");
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> thingDefFilterOptions = GetTypeFieldFilterOptions(tableWorker)
            .OrderBy(thingDef => thingDef?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new Widgets_Legacy.ThingDefIcon(thingDef))
            );
        Filter thingDefFilter = new OTMFilter<Verse.ThingDef?>((int row) => this[row].ThingDef, thingDefFilterOptions);
        int CompareByThingDefLabel(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].ThingDefLabel, this[row2].ThingDefLabel);
        CellField thingDefField = new(thingDefFieldLabel, thingDefFilter, CompareByThingDefLabel);

        return [countField, thingDefField];
    }
}
