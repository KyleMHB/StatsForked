using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class DefSetColumnWorker<TObject, TDef> : ColumnWorker<TObject, (HashSet<TDef> Defs, string? Text)> where TDef : Def
{
    protected DefSetColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    protected abstract HashSet<TDef> GetValue(TObject @object);
    protected virtual string GetDefLabel(TDef def)
    {
        return def.LabelCap;
    }
    protected sealed override DataCell GetCell(TObject @object)
    {
        var defs = GetValue(@object);

        if (defs.Count > 0)
        {
            var stringBuilder = new StringBuilder();

            foreach (var def in defs.OrderBy(GetDefLabel))
            {
                stringBuilder.AppendInNewLine(GetDefLabel(def));
            }

            var text = stringBuilder.ToString();
            var widget = new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            return new(widget, (defs, text));
        }

        return new(null, ([], null));
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> tableRecords)
    {
        var filterOptions = tableRecords
            .SelectMany(record => Cells[record].Data.Defs)
            .Distinct()
            .OrderBy(GetDefLabel)
            .Select<TDef, NTMFilterOption<TDef>>(
                def => def == null ? new() : new(def, GetDefLabel(def))
            );

        yield return new(ColumnDef.Title, Make.MTMFilter<TObject, TDef>(@object => Cells[@object].Data.Defs, filterOptions));
    }
    public sealed override int Compare(TObject object1, TObject object2)
    {
        return Comparer<string?>.Default.Compare(Cells[object1].Data.Text, Cells[object2].Data.Text);
    }
}
