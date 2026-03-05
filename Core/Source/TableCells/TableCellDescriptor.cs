using System;
using Stats.FilterWidgets;
using Stats.Widgets;
using UnityEngine;

namespace Stats.TableCells;

public readonly record struct TableCellDescriptor(TableCellStyleType Style, TableCellFieldDescriptor[] Fields);

public readonly record struct TableCellFieldDescriptor(Widget Label, FilterWidget FilterWidget, Comparison<int> Compare);

public enum TableCellStyleType
{
    String = TextAnchor.LowerLeft,
    Number = TextAnchor.LowerRight,
    Boolean = TextAnchor.LowerCenter,
}
