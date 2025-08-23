using System;
using System.Collections.Generic;
using Verse;

namespace Stats.Widgets;

public sealed partial class ObjectTable<TObject>
{
    private void HandleFilterChange(FilterWidget<TObject> filter)
    {
        if (filter.IsActive)
        {
            ActiveFilters.Add(filter);
        }
        else
        {
            ActiveFilters.Remove(filter);
        }

        DoFilter = true;
    }
    private void ApplyFilters()
    {
        foreach (var row in UnpinnedRows)
        {
            var rowIsValid = true;

            if (ActiveFilters.Count > 0)
            {
                try
                {
                    rowIsValid = ObjectMatchesFilters(row.Object, ActiveFilters);
                }
                catch (Exception e)
                {
                    Log.Error(e.Message);
                }
            }

            row.IsVisible = rowIsValid;
        }

        DoFilter = false;
        DoResize = true;
    }
    public override void ResetFilters()
    {
        if (ActiveFilters.Count == 0)
            return;

        foreach (var filter in Filters)
        {
            if (filter.IsActive)
            {
                filter.Reset();
            }
        }
    }
    public override void ToggleFilterMode()
    {
        FilterMode = FilterMode switch
        {
            TableFilterMode.AND => TableFilterMode.OR,
            TableFilterMode.OR => TableFilterMode.AND,
            _ => throw new NotSupportedException("Unsupported table filtering mode."),
        };
    }

    private delegate bool ObjectFilterMatchFunc(TObject @object, HashSet<FilterWidget<TObject>> filters);
}
