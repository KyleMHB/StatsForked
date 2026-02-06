using System;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;

namespace Stats.ObjectTable.Cells;

public readonly record struct CellDescriptor(CellStyleType Style, CellFieldDescriptor[] Fields);

public readonly record struct CellFieldDescriptor(Widget Label, FilterWidget FilterWidget, Comparison<Cell> Compare);

public enum CellStyleType
{
    Number = TextAnchor.LowerRight,
    String = TextAnchor.LowerLeft,
    Boolean = TextAnchor.LowerCenter,
}
