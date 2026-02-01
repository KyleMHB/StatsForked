using System;
using System.Collections.Generic;
using Stats.Objects.ThingDef;
using Stats.Widgets;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Pawn;

public sealed class Pawn_NameColumnWorker : ColumnWorker<VirtualThing>
{
    public Pawn_NameColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    public override ObjectTableWidget.Cell GetCell(VirtualThing thing)
    {
        return new Cell(thing);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<VirtualThing> _)
    {
        yield return new(Def.Title, new StringFilter<Cell>(cell => cell.Text, this));
    }

    private sealed class Cell : ObjectTableWidget.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public string Text { get; }
        public Cell(VirtualThing thing)
        {
            if (thing.Thing is Pawn pawn)
            {
                var text = pawn.Name.ToStringShort.CapitalizeFirst();
                var widget = new Label(text).PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);

                Widget = widget;
                Text = text;
            }
            else
            {
                Text = "";
            }
        }
        public override int CompareTo(ObjectTableWidget.Cell cell)
        {
            return Text.CompareTo(((Cell)cell).Text);
        }
    }
}
