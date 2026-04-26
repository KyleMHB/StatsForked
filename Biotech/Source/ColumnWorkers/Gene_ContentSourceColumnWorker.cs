using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Gene_ContentSourceColumnWorker(ColumnDef columnDef) : ColumnWorker<GeneDef, Gene_ContentSourceColumnWorker.GeneContentSourceCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override GeneContentSourceCell MakeCell(GeneDef geneDef)
    {
        ModContentPack? modContentPack = geneDef.modContentPack;
        if (modContentPack != null)
        {
            return new GeneContentSourceCell(modContentPack);
        }

        return default;
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<ModContentPack?>> valueFieldFilterOptions = ((IRefRecordsProvider<GeneDef>)tableWorker).Records
            .Select(geneDef => geneDef.modContentPack)
            .Distinct()
            .OrderBy(mod => mod?.Name)
            .Select(mod => mod == null ? new NTMFilterOption<ModContentPack?>() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing));
        Filter valueFieldFilter = new OTMFilter<ModContentPack?>((int row) => this[row].Mod, valueFieldFilterOptions);
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].ModName, this[row2].ModName);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, Compare);

        return [valueField];
    }

    public readonly struct GeneContentSourceCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly ModContentPack? Mod;
        public readonly string? ModName;

        private readonly TipSignal _tooltip;

        public GeneContentSourceCell(ModContentPack mod)
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
