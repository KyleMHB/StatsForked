using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefColumnWorker<TObject, TDef> : DefColumnWorker<TObject, TDef> where TDef : ThingDef?
{
    protected ThingDefColumnWorker(ColumnDef columnDef) : base(columnDef)
    {
    }
    protected sealed override Cell GetCell(TObject @object)
    {
        var def = GetValue(@object);

        if (def != null)
        {
            var widget = new HorizontalContainer([
                new ThingIcon(def).ToButtonGhostly(() => Draw.DefInfoDialog(def)),
                new Label(def.LabelCap),
            ], Globals.GUI.PadSm)
            .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, def);
        }

        return new();
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.OTMThingDefFilter(@object => Cells[@object].Data, tableRecords));
    }
}
