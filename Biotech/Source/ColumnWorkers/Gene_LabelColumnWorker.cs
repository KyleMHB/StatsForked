using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Gene_LabelColumnWorker : ColumnWorker<GeneDef, GeneDef>
{
    public Gene_LabelColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected override Cell GetCell(GeneDef geneDef)
    {
        var widget = new HorizontalContainer([
            new Icon(geneDef.Icon).ToButtonGhostly(() => Draw.DefInfoDialog(geneDef)),
            new Label(geneDef.LabelCap),
        ], Globals.GUI.Pad)
        .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer)
        .Tooltip(geneDef.description);

        return new(widget, geneDef);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<GeneDef> _)
    {
        yield return new(ColumnDef.Title, Make.StringFilter((GeneDef geneDef) => geneDef.label));
    }
    public override int Compare(GeneDef geneDef1, GeneDef geneDef2)
    {
        return geneDef1.label.CompareTo(geneDef2.label);
    }
}
