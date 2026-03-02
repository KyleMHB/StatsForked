using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using RimWorld;
using Stats.ObjectTable.FilterWidgets;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats.ObjectTable.Cells;

public readonly struct ThingDefCell : ICell
{
    public float Width { get; }
    public readonly ThingDef? Value;
    public readonly string Text = "";

    private readonly Widget? _icon;
    private readonly float _iconWidth;

    public ThingDefCell(ThingDef value)
    {
        Value = value;
        Text = value.LabelCap;
        _icon = new ThingDefIcon(value);
        float textWidth = Verse.Text.CalcSize(Text).x;
        float iconWidth = _icon.GetSize().x;
        Width = iconWidth + ObjectTableWidget.CellContentSpacing + textWidth;
        _iconWidth = iconWidth;
    }

    public void Draw(Rect rect)
    {
        if (Value != null && Event.current.type == EventType.Repaint)
        {
            rect = rect.ContractedByObjectTableCellPadding();

            _icon!.DrawIn(rect.CutByX(_iconWidth));

            rect.CutByX(ObjectTableWidget.CellContentSpacing);

            TextAnchor textAnchor = Verse.Text.Anchor;
            Verse.Text.Anchor = (TextAnchor)CellStyleType.String;

            Verse.Widgets.Label(rect, Text);

            Verse.Text.Anchor = textAnchor;
        }
    }

    //static private int CompareByCellText(Cell cell1, Cell cell2)
    //{
    //    return GetText(cell1).CompareTo(GetText(cell2));
    //}

    //static public CellDescriptor GetDescriptor(ColumnDef columnDef, IEnumerable<ThingDef?> defs) => GetDescriptor(columnDef.Title, defs);

    //static public CellDescriptor GetDescriptor(Widget valueFieldLabel, IEnumerable<ThingDef?> defs)
    //{
    //    IEnumerable<NTMFilterOption<ThingDef?>> valueFieldFilterOptions = defs
    //        .OrderBy(def => def?.label)
    //        .Select<ThingDef?, NTMFilterOption<ThingDef?>>(
    //            def => def == null ? new() : new(def, def.LabelCap)
    //        );
    //    FilterWidget valueFieldFilter = new OTMFilter<ThingDef?>(GetValue, valueFieldFilterOptions);
    //    CellFieldDescriptor valueField = new(valueFieldLabel, valueFieldFilter, CompareByCellText);

    //    return new CellDescriptor(CellStyleType.String, [valueField]);
    //}
}
