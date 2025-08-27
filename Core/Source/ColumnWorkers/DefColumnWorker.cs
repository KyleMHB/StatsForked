using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class DefColumnWorker<TObject, TDef> : ColumnWorker<TObject> where TDef : Def?
{
    protected DefColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String)
    {
    }
    protected abstract TDef GetValue(TObject @object);
    public sealed override ObjectTable.Cell GetCell(TObject @object)
    {
        var def = GetValue(@object);

        return new Cell(def);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> contextObjects)
    {
        var options = contextObjects
            .Select(GetValue)
            .Distinct()
            .OrderBy(def => def?.label)
            .Select<TDef, NTMFilterOption<TDef>>(
                def => def == null ? new() : new(def, def.LabelCap)
            );

        yield return new(ColumnDef.Title, new OTMFilter<Cell, TDef>(cell => cell.Def, options, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public TDef Def { get; }
        public string? DefLabel { get; }
        public Cell(TDef def)
        {
            Def = def;
            DefLabel = def?.label;

            if (def != null)
            {
                Widget = new Label(def.LabelCap).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Comparer<string?>.Default.Compare(DefLabel, ((Cell)cell).DefLabel);
        }
    }
}
