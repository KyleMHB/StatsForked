using System;
using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public abstract class NumberColumnWorker<TObject> : ColumnWorker<TObject>
{
    private readonly Texture2D? Icon = null;
    private readonly string FormatString;
    protected NumberColumnWorker(
        ColumnDef columndef,
        Texture2D? icon = null,
        string formatString = ""
    ) : base(columndef, CellStyleType.Number, TODO)
    {
        Icon = icon;
        FormatString = formatString;
    }
    protected abstract decimal GetValue(TObject @object);
    public sealed override ObjectTable.Cell GetCell(TObject @object)
    {
        var value = GetValue(@object);

        return new Cell(value, FormatString, Icon);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> _)
    {
        yield return new(Def.Title, new NumberFilter<Cell>(cell => cell.Value, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
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
