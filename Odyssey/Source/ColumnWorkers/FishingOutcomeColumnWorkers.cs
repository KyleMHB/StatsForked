using System.Collections.Generic;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Compat.Odyssey;

public abstract class FishingOutcomeTextColumnWorker(ColumnDef columnDef) : ColumnWorker<Def, FishingOutcomeTextColumnWorker.TextCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override TextCell MakeCell(Def def)
    {
        return new TextCell(GetText(def));
    }

    protected abstract string? GetText(Def def);

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter textFieldFilter = new StringFilter(row => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        return [new CellField(Def.TitleWidget, textFieldFilter, Compare)];
    }

    public readonly struct TextCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly string? Text;

        private readonly TipSignal _tooltip;

        public TextCell(string? text)
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

public abstract class FishingOutcomeNumberColumnWorker(ColumnDef columnDef, string formatString = "") : NumberColumnWorker<Def, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(Def def)
    {
        return TryGetValue(def, out decimal value) ? new NumberCell(value, formatString) : default;
    }

    protected abstract bool TryGetValue(Def def, out decimal value);
}

public sealed class FishingOutcomeLabelColumnWorker(ColumnDef columnDef) : FishingOutcomeTextColumnWorker(columnDef)
{
    protected override string? GetText(Def def)
    {
        return OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(def, "letterLabel")) ?? def.LabelCap.RawText;
    }
}

public sealed class FishingOutcomeFishTypeColumnWorker(ColumnDef columnDef) : FishingOutcomeTextColumnWorker(columnDef)
{
    protected override string? GetText(Def def)
    {
        return OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(def, "fishType"));
    }
}

public sealed class FishingOutcomeLetterColumnWorker(ColumnDef columnDef) : FishingOutcomeTextColumnWorker(columnDef)
{
    protected override string? GetText(Def def)
    {
        string? letterDef = OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(def, "letterDef"));
        string? letterText = OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(def, "letterText"));

        if (letterDef.NullOrEmpty())
        {
            return letterText;
        }

        if (letterText.NullOrEmpty())
        {
            return letterDef;
        }

        return $"{letterDef}: {letterText}";
    }
}

public sealed class FishingOutcomeDamageColumnWorker(ColumnDef columnDef) : FishingOutcomeTextColumnWorker(columnDef)
{
    protected override string? GetText(Def def)
    {
        string? damageDef = OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(def, "damageDef"));
        string? damageAmountRange = OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(def, "damageAmountRange"));

        if (damageDef.NullOrEmpty())
        {
            return damageAmountRange;
        }

        return damageAmountRange.NullOrEmpty() ? damageDef : $"{damageDef} ({damageAmountRange})";
    }
}

public sealed class FishingOutcomeHediffColumnWorker(ColumnDef columnDef) : FishingOutcomeTextColumnWorker(columnDef)
{
    protected override string? GetText(Def def)
    {
        return OdysseyReflection.ValueToString(OdysseyReflection.GetMemberValue(def, "addsHediff"));
    }
}

public sealed class FishingOutcomeSeverityColumnWorker(ColumnDef columnDef) : FishingOutcomeNumberColumnWorker(columnDef, "0.###")
{
    protected override bool TryGetValue(Def def, out decimal value)
    {
        return OdysseyReflection.TryGetDecimal(OdysseyReflection.GetMemberValue(def, "hediffSeverity"), out value);
    }
}

public sealed class FishingOutcomeContentSourceColumnWorker(ColumnDef columnDef) : OdysseyDefContentSourceColumnWorker(columnDef);

