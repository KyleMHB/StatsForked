using System;
using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefCountColumnWorker<TObject> : ColumnWorker<TObject>
{
    protected ThingDefCountColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.Number)
    {
        GetCachedValue = (TObject @object) =>
        {
            var value = GetValue(@object);

            if (value?.Count == 0m)
            {
                return null;
            }

            return value;
        };
        GetCachedValue = GetCachedValue.Memoized();
    }
    protected readonly Func<TObject, ThingDefCount?> GetCachedValue;
    protected abstract ThingDefCount? GetValue(TObject @object);
    public sealed override Widget? GetTableCellWidget(TObject @object)
    {
        var thingDefCount = GetCachedValue(@object);

        if (thingDefCount == null)
        {
            return null;
        }

        var count = thingDefCount.Value.Count;
        var thingDef = thingDefCount.Value.Def;

        return new HorizontalContainer(
            [
                new Label(count.ToString()).PaddingRel(1f, 0f, 0f, 0f),
                new ThingIcon(thingDef).ToButtonGhostly(() => Draw.DefInfoDialog(thingDef), thingDef.LabelCap),
            ],
            Globals.GUI.PadSm,
            true
        );
    }
    public override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords)
    {
        var countFilter = Make.NumberFilter<TObject>(@object => GetCachedValue(@object)?.Count ?? 0m).Tooltip("Filter by amount");
        var typeFilter = Make.OTMThingDefFilter(@object => GetCachedValue(@object)?.Def, tableRecords).Tooltip("Filter by type");

        return Make.CompositeFilter<TObject>([countFilter, typeFilter], true);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        var count1 = GetCachedValue(object1)?.Count;
        var count2 = GetCachedValue(object2)?.Count;

        return Comparer<decimal?>.Default.Compare(count1, count2);
    }

    protected readonly record struct ThingDefCount(ThingDef Def, decimal Count);
}
