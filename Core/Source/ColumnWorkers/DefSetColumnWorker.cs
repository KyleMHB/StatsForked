using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class DefSetColumnWorker<TObject, TDef> : ColumnWorker<TObject> where TDef : Def
{
    protected DefSetColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    protected abstract HashSet<TDef> GetValue(TObject @object);
    protected virtual string GetDefLabel(TDef def)
    {
        return def.LabelCap;
    }
    public sealed override ObjectTable.Cell GetCell(TObject @object)
    {
        var defs = GetValue(@object);

        return new Cell(defs, this);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> contextObjects)
    {
        var filterOptions = contextObjects
            .SelectMany(GetValue)
            .Distinct()
            .OrderBy(GetDefLabel)
            .Select<TDef, NTMFilterOption<TDef>>(
                def => def == null ? new() : new(def, GetDefLabel(def))
            );

        yield return new(Def.Title, new MTMFilter<Cell, TDef>(cell => cell.Defs, filterOptions, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public HashSet<TDef> Defs { get; }
        public string? Text { get; }
        public Cell(HashSet<TDef> defs, DefSetColumnWorker<TObject, TDef> column)
        {
            Defs = defs;

            if (defs.Count > 0)
            {
                var stringBuilder = new StringBuilder();

                foreach (var def in defs.OrderBy(column.GetDefLabel))
                {
                    stringBuilder.AppendInNewLine(column.GetDefLabel(def));
                }

                var text = stringBuilder.ToString();
                Widget = new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
                Text = text;
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Comparer<string?>.Default.Compare(Text, ((Cell)cell).Text);
        }
    }
}
