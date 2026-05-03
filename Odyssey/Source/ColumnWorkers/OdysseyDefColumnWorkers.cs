using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Compat.Odyssey;

public abstract class OdysseyDefLabelColumnWorker(ColumnDef columnDef) : ColumnWorker<Def, OdysseyDefLabelColumnWorker.TextCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override TextCell MakeCell(Def def)
    {
        return new TextCell(def.LabelCap.RawText);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter textFieldFilter = new StringFilter((int row) => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField textField = new(Def.TitleWidget, textFieldFilter, Compare);

        return [textField];
    }

    public readonly struct TextCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly string? Text;

        public TextCell(string text)
        {
            Text = text;
            Width = Verse.Text.CalcSize(text).x;
        }

        public void Draw(Rect rect)
        {
            if (Text != null)
            {
                rect.Label(Text, GUIStyles.TableCell.String);
            }
        }
    }
}

public abstract class OdysseyDefDescriptionColumnWorker(ColumnDef columnDef) : ColumnWorker<Def, OdysseyDefDescriptionColumnWorker.DescriptionCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override DescriptionCell MakeCell(Def def)
    {
        return new DescriptionCell(def.description);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter textFieldFilter = new StringFilter((int row) => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField textField = new(Def.TitleWidget, textFieldFilter, Compare);

        return [textField];
    }

    public readonly struct DescriptionCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly string? Text;

        private readonly TipSignal _tooltip;

        public DescriptionCell(string? text)
        {
            Text = text;
            string preview = text?.Truncate(80) ?? "";
            Width = Verse.Text.CalcSize(preview).x;
            _tooltip = text ?? "";
        }

        public void Draw(Rect rect)
        {
            if (Text != null)
            {
                rect
                    .Label(Text.Truncate(80), GUIStyles.TableCell.String)
                    .Tip(_tooltip);
            }
        }
    }
}

public abstract class OdysseyDefContentSourceColumnWorker(ColumnDef columnDef) : ColumnWorker<Def, OdysseyDefContentSourceColumnWorker.ModContentPackCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override ModContentPackCell MakeCell(Def def)
    {
        ModContentPack? modContentPack = def.modContentPack;
        if (modContentPack != null)
        {
            return new ModContentPackCell(modContentPack);
        }

        return default;
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<ModContentPack?>> valueFieldFilterOptions = ((IRefRecordsProvider<Def>)tableWorker).Records
            .Select(def => def.modContentPack)
            .Distinct()
            .OrderBy(mod => mod?.Name)
            .Select(mod => mod == null ? new NTMFilterOption<ModContentPack?>() : new(mod, mod.Name, null, mod.PackageIdPlayerFacing));
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
            Width = Verse.Text.CalcSize(ModName).x;
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
