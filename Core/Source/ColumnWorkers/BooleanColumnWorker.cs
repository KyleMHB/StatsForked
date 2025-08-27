using System;
using System.Collections.Generic;
using Stats.Widgets;

namespace Stats;

public abstract class BooleanColumnWorker<TObject> : ColumnWorker<TObject>
{
    protected BooleanColumnWorker(ColumnDef columndef) : base(columndef, CellStyleType.Boolean)
    {
    }
    protected abstract bool GetValue(TObject @object);
    public sealed override ObjectTable.Cell GetCell(TObject @object)
    {
        var value = GetValue(@object);

        return new Cell(value);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> _)
    {
        yield return new(ColumnDef.Title, new BooleanFilter<Cell>(cell => cell.Value, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public bool Value { get; }
        public Cell(bool value)
        {
            Value = value;

            if (value == true)
            {
                Widget = new Icon(Verse.Widgets.CheckboxOnTex)
                .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Value.CompareTo(((Cell)cell).Value);
        }
    }
}
