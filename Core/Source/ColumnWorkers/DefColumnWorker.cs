using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class DefColumnWorker<TObject, TDef> : ColumnWorker<TObject, TDef> where TDef : Def?
{
    protected DefColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected abstract TDef GetValue(TObject @object);
    protected override DataCell GetCell(TObject @object)
    {
        var def = GetValue(@object);

        if (def != null)
        {
            var widget = new Label(def.LabelCap).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, def);
        }

        return new(null, null);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.OTMDefFilter(@object => Cells[@object].Data, tableRecords));
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        var defLabel1 = Cells[object1].Data?.label;
        var defLabel2 = Cells[object2].Data?.label;

        return Comparer<string?>.Default.Compare(defLabel1, defLabel2);
    }
}
