using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class DefSetColumnWorker<TObject, TValue> : ColumnWorker<TObject> where TValue : Def
{
    protected DefSetColumnWorker(ColumnDef columnDef, bool cached = true) : base(columnDef, ColumnCellStyle.String)
    {
        GetCachedValue = GetValue;

        if (cached)
        {
            GetCachedValue = GetCachedValue.Memoized();
        }

        GetDefLabels = FunctionExtensions.Memoized((HashSet<TValue> defs) =>
        {
            if (defs.Count == 0)
            {
                return "";
            }

            var stringBuilder = new StringBuilder();

            foreach (var def in defs.OrderBy(GetDefLabel))
            {
                stringBuilder.AppendInNewLine(GetDefLabel(def));
            }

            return stringBuilder.ToString();
        });
    }
    protected readonly Func<TObject, HashSet<TValue>> GetCachedValue;
    protected abstract HashSet<TValue> GetValue(TObject @object);
    private readonly Func<HashSet<TValue>, string> GetDefLabels;
    protected virtual string GetDefLabel(TValue def)
    {
        return def.LabelCap;
    }
    public override Widget? GetTableCellWidget(TObject @object)
    {
        var defLabels = GetDefLabels(GetCachedValue(@object));

        if (defLabels.Length == 0)
        {
            return null;
        }

        return new Label(defLabels);
    }
    public override FilterWidget<TObject> GetFilterWidget(IEnumerable<TObject> tableRecords)
    {
        var filterOptions = tableRecords
            .SelectMany(GetCachedValue)
            .Distinct()
            .OrderBy(GetDefLabel)
            .Select<TValue, NTMFilterOption<TValue>>(
                def => def == null ? new() : new(def, GetDefLabel(def))
            );

        return Make.MTMFilter(GetCachedValue, filterOptions);
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return GetDefLabels(GetCachedValue(object1)).CompareTo(GetDefLabels(GetCachedValue(object2)));
    }
}
