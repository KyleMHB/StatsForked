using System;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ThingDefColumnWorker<TObject, TDef> : ColumnWorker<TObject> where TDef : ThingDef?
{
    protected ThingDefColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    protected abstract TDef GetValue(TObject @object);
    public sealed override ObjectTableWidget.Cell GetCell(TObject @object)
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

        yield return new(Def.Title, new OTMFilter<Cell, TDef>(cell => cell.Def, options, this));
    }

    private sealed class Cell : ObjectTableWidget.WidgetCell
    {
        protected override Widget? Widget { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public override event Action? OnChange;
        public TDef Def { get; }
        public string? DefLabel { get; }
        public Cell(TDef def)
        {
            Def = def;
            DefLabel = def?.label;

            if (def != null)
            {
                Widget = new HorizontalContainer([
                    new ThingDefIcon(def).ToButtonGhostly(() => Widgets.Draw.DefInfoDialog(def)),
                    new Label(def.LabelCap),
                ], Globals.GUI.PadSm)
                .PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
            }
        }
        public override int CompareTo(ObjectTableWidget.Cell cell)
        {
            return Comparer<string?>.Default.Compare(DefLabel, ((Cell)cell).DefLabel);
        }
    }
}
