using System;
using Stats.Filters;
using Stats.Widgets_Legacy;

namespace Stats.ColumnWorkers;

public readonly record struct CellField(Widget Label, FilterWidget FilterWidget, Comparison<int> Compare);
