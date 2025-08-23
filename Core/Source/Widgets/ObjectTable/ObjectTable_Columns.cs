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
    private void RefreshColumns()
    {
        var anyColumnWasUpdated = false;

        foreach (var column in Columns)
        {
            anyColumnWasUpdated |= column.Refresh();
        }

        if (anyColumnWasUpdated)
        {
            DoFilter = true;
            DoSort = true;
            DoResize = true;
        }

        DoRefreshColumns = false;
    }
}
