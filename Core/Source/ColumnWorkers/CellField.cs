using System;
using Stats.Filters;
using Stats.Widgets_Legacy;

namespace Stats.ColumnWorkers;

public readonly record struct CellField(Widget Label, Filter FilterWidget, Comparison<int> Compare);
