using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.Def;

public sealed class ModContentPackColumnWorker(ColumnDef columnDef) : ColumnWorker<DefBasedObject, ModContentPackColumnWorker.ModContentPackCell>
{
    public override ColumnType Type => ColumnType.String;
    public override ColumnDef Def => columnDef;

    protected override ModContentPackCell MakeCell(DefBasedObject @object)
    {
        ModContentPack? modContentPack = @object.Def.modContentPack;

        if (modContentPack != null)
        {
            return new ModContentPackCell(modContentPack);
        }

        return default;
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<ModContentPack?>> valueFieldFilterOptions = ((IRefRecordsProvider<Verse.Def>)tableWorker).Records
            .Select(def => def.modContentPack)
            .Distinct()
            .OrderBy(mod => mod?.Name)
            .Select<ModContentPack?, NTMFilterOption<ModContentPack?>>(
                mod => mod == null ? new() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing)
            );
        Filter valueFieldFilter = new OTMFilter<ModContentPack?>((int row) => this[row].Mod, valueFieldFilterOptions);
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].ModName, this[row2].ModName);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, Compare);

        return [valueField];
    }

    public readonly struct ModContentPackCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly ModContentPack? Mod;
        public readonly string? ModName;

        private readonly TipSignal _tooltip;

        public ModContentPackCell(ModContentPack mod)
        {
            Mod = mod;
            ModName = mod.Name;
            Width = Text.CalcSize(ModName).x;
            _tooltip = mod.PackageIdPlayerFacing;
        }

        public void Draw(Rect rect)
        {
            if (ModName != null)
            {
                rect
                    .Label(ModName, GUIStyles.TableCell.String)
                    .Tip(_tooltip);
            }
        }
    }
}
