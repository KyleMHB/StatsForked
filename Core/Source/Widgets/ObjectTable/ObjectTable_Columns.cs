namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    private void UpdateCachedColumns()
    {
        ColumnsVisible.Clear();
        ColumnsVisiblePinned.Clear();
        ColumnsVisibleUnpinned.Clear();

        foreach (var column in Columns)
        {
            if (column.IsVisible)
            {
                ColumnsVisible.Add(column);

                if (column.IsPinned)
                {
                    ColumnsVisiblePinned.Add(column);
                }
                else
                {
                    ColumnsVisibleUnpinned.Add(column);
                }
            }
        }

        DoUpdateCachedColumns = false;
    }
}
