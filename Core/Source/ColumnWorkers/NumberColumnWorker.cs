using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public abstract class NumberColumnWorker : ColumnWorker
{
    private readonly Texture2D? Icon = null;
    protected NumberColumnWorker(
        ColumnDef columndef,
        Texture2D? icon = null
    ) : base(columndef, IColumnWorker.CellStyleType.Number)
    {
        Icon = icon;
    }
    protected ObjectTable.Cell GetCell(decimal value, string formatString)
    {
        return new Cell(value, formatString, Icon);
    }
    protected IEnumerable<ObjectTable.ObjectProp> GetObjectProps()
    {
        yield return new(Def.Title, new NumberFilter(cell => ((Cell)cell).Value));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public decimal Value { get; }
        public Cell(decimal value, string formatString, Texture2D? icon)
        {
            Value = value;

            if (value != 0m)
            {
                Widget widget = new Label(value.ToString(formatString));

                if (icon != null)
                {
                    widget = new HorizontalContainer([
                        widget.WidthRel(1f),
                        new Icon(icon)
                    ], Globals.GUI.PadSm, true);
                }

                Widget = widget.PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Value.CompareTo(((Cell)cell).Value);
        }
    }
}
