using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class EquippedStatOffsetsColumnWorker(ColumnDef columnDef)
    : ColumnWorker<DefBasedObject, EquippedStatOffsetsColumnWorker.OffsetsCell>
{
    public override ColumnType Type => ColumnType.String;
    public override ColumnDef Def => columnDef;

    protected override OffsetsCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is not Verse.ThingDef thingDef || thingDef.equippedStatOffsets.NullOrEmpty())
        {
            return default;
        }

        List<StatModifier> statOffsets = thingDef.equippedStatOffsets
            .OrderBy(statMod => statMod.stat.LabelCap.RawText)
            .ToList();

        return new OffsetsCell(statOffsets);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter textFieldFilter = new StringFilter((int row) => this[row].Text ?? "");
        int CompareText(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField textField = new(Def.TitleWidget, textFieldFilter, CompareText);

        return [textField];
    }

    public readonly struct OffsetsCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly string? Text;

        private readonly TipSignal _tooltip;

        public OffsetsCell(IReadOnlyList<StatModifier> statOffsets)
        {
            Text = null;
            Width = 0f;
            _tooltip = default;

            if (statOffsets.Count == 0)
            {
                return;
            }

            string firstOffsetText = StatOffsetToString(statOffsets[0]);
            int hiddenOffsetsCount = statOffsets.Count - 1;
            Text = hiddenOffsetsCount > 0
                ? $"{firstOffsetText} +{hiddenOffsetsCount}"
                : firstOffsetText;
            Width = Text.CalcSize(GUIStyles.TableCell.StringNoPad).x;
            _tooltip = string.Join("\n", statOffsets.Select(StatOffsetToString));
        }

        public void Draw(Rect rect)
        {
            if (Text != null)
            {
                rect
                    .Label(Text, GUIStyles.TableCell.String)
                    .Tip(_tooltip);
            }
        }

        private static string StatOffsetToString(StatModifier statMod)
        {
            return $"{statMod.stat.LabelCap}: {statMod.stat.ValueToString(statMod.value, ToStringNumberSense.Offset, statMod.stat.finalizeEquippedStatOffset)}";
        }
    }
}
