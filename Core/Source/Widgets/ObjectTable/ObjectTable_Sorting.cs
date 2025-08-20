using System.Collections.Generic;

namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    private void SortRows(List<ObjectRow> rows)
    {
        rows.Sort(CompareRows);
    }
    private int CompareRows(ObjectRow r1, ObjectRow r2)
    {
        var result = SortColumn.Compare(r1.Object, r2.Object) * SortDirection;

        if (result == 0)
        {
            result = r1.Object.GetHashCode().CompareTo(r2.Object.GetHashCode());
        }

        return result;
    }
}
