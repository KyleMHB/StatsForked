using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class ContentSourceColumnWorker<TObject> : ColumnWorker<TObject>
{
    protected ContentSourceColumnWorker(ColumnDef columndef) : base(columndef, CellStyleType.String, TODO)
    {
    }
    protected abstract ModContentPack? GetModContentPack(TObject @object);
    public sealed override ObjectTable.Cell GetCell(TObject @object)
    {
        var mod = GetModContentPack(@object);

        return new Cell(mod);
    }
    public sealed override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<TObject> contextObjects)
    {
        var filterOptions = contextObjects
            .Select(GetModContentPack)
            .Distinct()
            .OrderBy(mod => mod?.Name)
            .Select<ModContentPack?, NTMFilterOption<ModContentPack?>>(
                mod => mod == null ? new() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing)
            );

        yield return new(Def.Title, new OTMFilter<Cell, ModContentPack?>(cell => cell.Mod, filterOptions, this));
    }

    private sealed class Cell : ObjectTable.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public ModContentPack? Mod { get; }
        public string? ModName { get; }
        public Cell(ModContentPack? mod)
        {
            Mod = mod;
            ModName = mod?.Name;

            if (mod != null)
            {
                Widget = new Label(mod.Name)
                .PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer)
                .Tooltip(mod.PackageIdPlayerFacing);
            }
        }
        public override int CompareTo(ObjectTable.Cell cell)
        {
            return Comparer.Default.Compare(ModName, ((Cell)cell).ModName);
        }
    }
}
