using System;
using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class DefColumnWorker<TObject, TValue> : ColumnWorker<TObject> where TValue : Def?
{
    protected DefColumnWorker(ColumnDef columnDef, bool cached = true) : base(columnDef, ColumnCellStyle.String)
    {
        GetCachedValue = GetValue;

        if (cached)
        {
            GetCachedValue = GetCachedValue.Memoized();
        }
    }
    protected readonly Func<TObject, TValue> GetCachedValue;
    protected abstract TValue GetValue(TObject @object);
    public override Widget? GetTableCellWidget(TObject @object)
    {
        var def = GetCachedValue(@object);

        if (def == null)
        {
            return null;
        }

        return new Label(def.LabelCap);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.OTMDefFilter(GetCachedValue, tableRecords));
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        var defLabel1 = GetCachedValue(object1)?.label;
        var defLabel2 = GetCachedValue(object2)?.label;

        return Comparer<string?>.Default.Compare(defLabel1, defLabel2);
    }
}
