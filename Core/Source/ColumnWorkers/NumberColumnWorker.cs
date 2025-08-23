using System.Collections.Generic;
using Stats.Widgets;
using UnityEngine;

namespace Stats;

public abstract class NumberColumnWorker<TObject> : ColumnWorker<TObject, decimal>
{
    private readonly Texture2D? Icon = null;
    private readonly string FormatString;
    protected NumberColumnWorker(
        ColumnDef columndef,
        Texture2D? icon = null,
        string formatString = ""
    ) : base(columndef, ColumnCellStyle.Number)
    {
        Icon = icon;
        FormatString = formatString;
    }
    protected abstract decimal GetValue(TObject @object);
    protected sealed override Cell GetCell(TObject @object)
    {
        var value = GetValue(@object);

        if (value != 0m)
        {
            Widget cellWidget = new Label(value.ToString(FormatString));

            if (Icon != null)
            {
                cellWidget = new HorizontalContainer(
                    [
                        cellWidget.WidthRel(1f),
                        new Icon(Icon)
                    ],
                    Globals.GUI.PadSm,
                    true
                );
            }

            return new(cellWidget.PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer), value);
        }

        return new(null, 0m);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> _)
    {
        yield return new(ColumnDef.Title, Make.NumberFilter<TObject>(@object => Cells[@object].Data));
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return Cells[object1].Data.CompareTo(Cells[object2].Data);
    }
}
