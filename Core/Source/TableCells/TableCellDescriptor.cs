using System;
using Stats.FilterWidgets;
using Stats.Widgets;
using UnityEngine;

namespace Stats.TableCells;

public readonly record struct TableCellDescriptor(TableCellStyleType Style, TableCellFieldDescriptor[] Fields);

public readonly record struct TableCellFieldDescriptor(Widget Label, FilterWidget FilterWidget, Comparison<Cell> Compare);

public enum TableCellStyleType
{
    Number = TextAnchor.LowerRight,
    String = TextAnchor.LowerLeft,
    Boolean = TextAnchor.LowerCenter,
}
