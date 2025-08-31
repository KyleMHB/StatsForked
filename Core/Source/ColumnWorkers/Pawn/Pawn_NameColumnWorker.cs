using System;
using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Pawn_NameColumnWorker : ColumnWorker<ThingAlike>
{
    public Pawn_NameColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    public override ObjectTable.Cell GetCell(ThingAlike thing)
    {
        return new Cell(thing);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> _)
    {
        yield return new(Def.Title, new StringFilter<Cell>(cell => cell.Text, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public string Text { get; }
        public Cell(ThingAlike thing)
        {
            if (thing.Thing is Pawn pawn)
            {
                var text = pawn.Name.ToStringShort.CapitalizeFirst();
                var widget = new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

                Widget = widget;
                Text = text;
            }
            else
            {
                Text = "";
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Text.CompareTo(((Cell)cell).Text);
        }
    }
}
