using System.Collections.Generic;
using Stats.Widgets;

namespace Stats;

public abstract class BooleanColumnWorker<TObject> : ColumnWorker<TObject, bool>
{
    protected BooleanColumnWorker(ColumnDef columndef) : base(columndef, ColumnCellStyle.Boolean)
    {
    }
    protected abstract bool GetValue(TObject @object);
    protected sealed override Cell GetCell(TObject @object)
    {
        var value = GetValue(@object);

        if (value == true)
        {
            var widget = new Icon(Verse.Widgets.CheckboxOnTex)
            .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, true);
        }

        return new();
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> _)
    {
        yield return new(ColumnDef.Title, Make.BooleanFilter<TObject>(@object => Cells[@object].Data));
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return Cells[object1].Data.CompareTo(Cells[object2].Data);
    }
}
