using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Stats.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.TableCells;

public readonly struct DefSetTableCell : ITableCell
{
    private static readonly HashSet<Def> EmptyDefHashSet = [];

    public float Width { get; }
    public readonly IReadOnlyCollection<Def> Value = EmptyDefHashSet;
    public readonly string Text = "";

    public DefSetTableCell(IReadOnlyCollection<Def> value)
    {
        Value = value;
        if (value.Count > 0)
        {
            Text = string.Join("\n", Value.Select(def => def.LabelCap).OrderBy(text => text));
            Width = Verse.Text.CalcSize(Text).x;
        }
    }

    public void Draw(Rect rect)
    {
        if (Text.Length > 0 && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            TextAnchor textAnchor = Verse.Text.Anchor;
            Verse.Text.Anchor = (TextAnchor)TableCellStyleType.String;

            Verse.Widgets.Label(rect, Text);

            Verse.Text.Anchor = textAnchor;
        }
    }

    //static private int CompareByCellText(Cell cell1, Cell cell2)
    //{
    //    return ((Constant)cell1).Text.CompareTo(((Constant)cell2).Text);
    //}

    //static public CellDescriptor GetDescriptor(ColumnDef columnDef, IEnumerable<Def?> defs) => GetDescriptor(columnDef.Title, defs);

    //static public CellDescriptor GetDescriptor(Widget valueFieldLabel, IEnumerable<Def?> defs)
    //{
    //    IEnumerable<NTMFilterOption<Def?>> valueFieldFilterOptions = defs
    //        .OrderBy(def => def?.label)
    //        .Select<Def?, NTMFilterOption<Def?>>(
    //            def => def == null ? new() : new(def, def.LabelCap)
    //        );
    //    FilterWidget valueFieldFilter = new MTMFilter<Def?>(GetValue, valueFieldFilterOptions);
    //    CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, CompareByCellText);

    //    return new CellDescriptor(CellStyleType.String, [valueField]);
    //}
}
