using System.Collections.Generic;

namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    private void SortRows()
    {
        SortRows(PinnedRows);
        SortRows(UnpinnedRows);

        DoSort = false;
    }
    private void SortRows(List<ObjectRow> rows)
    {
        rows.Sort(CompareRows);
    }
    private int CompareRows(ObjectRow r1, ObjectRow r2)
    {
        return r1.CompareToByColumn(r2, SortColumn) * SortDirection;
    }
}
