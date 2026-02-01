using System;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;

namespace Stats.ObjectTable.Cells;

public readonly record struct CellFieldDescriptor(Widget Label, FilterWidget FilterWidget, Comparison<Cell> Compare);
