using System.Collections.Generic;
using System.Linq;
using Stats.FilterWidgets;
using Stats.TableCells;
using Stats.TableWorkers;
using Stats.Widgets;

namespace Stats.ColumnWorkers;

public abstract class ThingDefCountColumnWorker<TObject, TCell> : ColumnWorker<TObject, TCell> where TCell : struct, IThingDefCountTableCell
{
    protected abstract IEnumerable<Verse.ThingDef?> GetTypeFieldFilterOptions(TableWorker tableWorker);

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        Widget countFieldLabel = new Label("Amount");
        FilterWidget countFilter = new NumberFilter((int row) => this[row].Count);
        int CompareByCount(int row1, int row2) => this[row1].Count.CompareTo(this[row2].Count);
        TableCellFieldDescriptor countField = new(countFieldLabel, countFilter, CompareByCount);

        Widget thingDefFieldLabel = new Label("Type");
        IEnumerable<NTMFilterOption<Verse.ThingDef?>> thingDefFilterOptions = GetTypeFieldFilterOptions(tableWorker)
            .OrderBy(thingDef => thingDef?.label)
            .Select<Verse.ThingDef?, NTMFilterOption<Verse.ThingDef?>>(
                thingDef => thingDef == null
                    ? new()
                    : new(thingDef, thingDef.LabelCap, new ThingDefIcon(thingDef))
            );
        FilterWidget thingDefFilter = new OTMFilter<Verse.ThingDef?>((int row) => this[row].ThingDef, thingDefFilterOptions);
        int CompareByThingDefLabel(int row1, int row2) => this[row1].ThingDefLabel.CompareTo(this[row2].ThingDefLabel);
        TableCellFieldDescriptor thingDefField = new(thingDefFieldLabel, thingDefFilter, CompareByThingDefLabel);

        return new TableCellDescriptor(TableCellStyleType.Number, [countField, thingDefField]);
    }
}
