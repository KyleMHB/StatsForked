using System;
using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Gene_LabelColumnWorker : ColumnWorker<GeneDef>
{
    public Gene_LabelColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    public override ObjectTableWidget.Cell GetCell(GeneDef geneDef)
    {
        return new Cell(geneDef);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<GeneDef> _)
    {
        yield return new(Def.Title, new StringFilter<Cell>(cell => cell.Text, this));
    }

    private sealed class Cell : ObjectTableWidget.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public string Text { get; }
        public Cell(GeneDef geneDef)
        {
            Text = geneDef.label;
            Widget = new HorizontalContainer([
                new Icon(geneDef.Icon).ToButtonGhostly(() => Widgets.Draw.DefInfoDialog(geneDef)),
                new Label(geneDef.LabelCap),
            ], Globals.GUI.Pad)
            .PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer)
            .Tooltip(geneDef.description);
        }
        public override int CompareTo(ObjectTableWidget.Cell cell)
        {
            return Text.CompareTo(((Cell)cell).Text);
        }
    }
}
