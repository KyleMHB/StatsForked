using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using Stats.Utils;
using Stats.Utils.Extensions;
using Stats.Utils.Widgets;
using UnityEngine;
using Verse;

namespace Stats.Bionics;

public sealed class BionicLabelColumnWorker(ColumnDef columnDef) : ColumnWorker<BionicOperation, BionicLabelColumnWorker.Cell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override Cell MakeCell(BionicOperation @object)
    {
        return new Cell(@object.ThingDef, @object.DisplayLabel);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filters.Filter filter = new Filters.StringFilter(row => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        return [new CellField(Def.TitleWidget, filter, Compare)];
    }

    public readonly struct Cell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public ThingDef? Value { get; }
        public string? Text { get; }

        private readonly ThingDefIcon? _icon;
        private readonly float _iconWidth;

        public Cell(ThingDef? thingDef, string label)
        {
            Value = thingDef;
            Text = label;
            float textWidth = Verse.Text.CalcSize(label).x;
            if (thingDef != null)
            {
                _icon = new ThingDefIcon(thingDef);
                _iconWidth = _icon.Size.x;
                Width = _iconWidth + GUIStyles.TableCell.ContentSpacing + textWidth;
            }
            else
            {
                Width = textWidth;
            }
        }

        public void Draw(Rect rect)
        {
            if (Text == null)
            {
                return;
            }

            if (Value == null || _icon == null)
            {
                rect.Label(Text, GUIStyles.TableCell.String);
                return;
            }

            rect
                .ContractedByObjectTableCellPadding()
                .CutLeft(out Rect iconRect, _iconWidth)
                .CutLeft(GUIStyles.TableCell.ContentSpacing)
                .TakeRest(out Rect labelRect);

            if (Event.current.type == EventType.Repaint)
            {
                _icon.Draw(iconRect);
                Text.Draw(labelRect, GUIStyles.TableCell.StringNoPad);
            }

            if (iconRect.ButtonGhostly())
            {
                Value.OpenInfoDialog();
            }
        }
    }
}

public sealed class BionicBodyPartsColumnWorker(ColumnDef columnDef) : DefSetColumnWorker<BionicOperation, DefSetCell>
{
    public override ColumnDef Def => columnDef;

    protected override DefSetCell MakeCell(BionicOperation @object)
    {
        return new DefSetCell(@object.BodyParts.Cast<Def>().ToArray());
    }

    protected override IEnumerable<Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((TableWorker<BionicOperation>)tableWorker).InitialObjects
            .SelectMany(operation => operation.BodyParts)
            .Distinct();
    }
}

public sealed class BionicCapacitiesColumnWorker(ColumnDef columnDef) : DefSetColumnWorker<BionicOperation, DefSetCell>
{
    public override ColumnDef Def => columnDef;

    protected override DefSetCell MakeCell(BionicOperation @object)
    {
        return new DefSetCell(@object.Capacities.Cast<Def>().ToArray());
    }

    protected override IEnumerable<Def?> GetValueFieldFilterOptions(TableWorker tableWorker)
    {
        return ((TableWorker<BionicOperation>)tableWorker).InitialObjects
            .SelectMany(operation => operation.Capacities)
            .Distinct();
    }
}

public sealed class BionicEfficiencyColumnWorker(ColumnDef columnDef) : NumberColumnWorker<BionicOperation, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(BionicOperation @object)
    {
        return @object.Efficiency == 0m ? default : new NumberCell(@object.Efficiency, "0.00");
    }
}

public sealed class BionicEffectsColumnWorker(ColumnDef columnDef) : ColumnWorker<BionicOperation, TextCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override TextCell MakeCell(BionicOperation @object)
    {
        return new TextCell(@object.EffectsSummary);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filters.Filter filter = new Filters.StringFilter(row => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        return [new CellField(Def.TitleWidget, filter, Compare)];
    }
}

public sealed class BionicSpecialColumnWorker(ColumnDef columnDef) : ColumnWorker<BionicOperation, TextCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override TextCell MakeCell(BionicOperation @object)
    {
        return new TextCell(@object.SpecialEffects.Count == 0 ? null : string.Join(", ", @object.SpecialEffects));
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filters.Filter filter = new Filters.StringFilter(row => this[row].Text ?? "");
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        return [new CellField(Def.TitleWidget, filter, Compare)];
    }
}

public abstract class BionicEffectValueColumnWorker(ColumnDef columnDef, string effectKey, string formatString) : NumberColumnWorker<BionicOperation, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberCell MakeCell(BionicOperation @object)
    {
        return @object.TryGetEffect(effectKey, out BionicEffectValue? effect)
            ? new NumberCell(effect.Value, formatString)
            : default;
    }
}

public sealed class BionicConsciousnessColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Consciousness, "+0;-0;0") { }
public sealed class BionicMovingColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Moving, "+0;-0;0\\%") { }
public sealed class BionicManipulationColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Manipulation, "+0;-0;0\\%") { }
public sealed class BionicTalkingColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Talking, "+0;-0;0\\%") { }
public sealed class BionicSightColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Sight, "+0;-0;0\\%") { }
public sealed class BionicHearingColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Hearing, "+0;-0;0\\%") { }
public sealed class BionicBreathingColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Breathing, "+0;-0;0\\%") { }
public sealed class BionicBloodFiltrationColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.BloodFiltration, "+0;-0;0\\%") { }
public sealed class BionicBloodPumpingColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.BloodPumping, "+0;-0;0\\%") { }
public sealed class BionicMetabolismColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Metabolism, "+0;-0;0\\%") { }
public sealed class BionicBeautyColumnWorker(ColumnDef columnDef) : BionicEffectValueColumnWorker(columnDef, BionicEffectKeys.Beauty, "+0.##;-0.##;0") { }

public sealed class BionicContentSourceColumnWorker(ColumnDef columnDef) : ColumnWorker<BionicOperation, BionicContentSourceColumnWorker.Cell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override Cell MakeCell(BionicOperation @object)
    {
        return new Cell(@object.Recipe.modContentPack);
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<Filters.NTMFilterOption<ModContentPack?>> options = ((TableWorker<BionicOperation>)tableWorker).InitialObjects
            .Select(operation => operation.Recipe.modContentPack)
            .Distinct()
            .Select(pack => new Filters.NTMFilterOption<ModContentPack?>(pack, pack?.Name ?? "Unknown"));
        Filters.Filter filter = new Filters.OTMFilter<ModContentPack?>(row => this[row].Value, options);
        int Compare(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        return [new CellField(Def.TitleWidget, filter, Compare)];
    }

    public readonly struct Cell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public string? Text { get; }
        public ModContentPack? Value { get; }

        public Cell(ModContentPack? value)
        {
            Value = value;
            Text = value?.Name;
            Width = Text == null ? 0f : Verse.Text.CalcSize(Text).x;
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

public readonly struct TextCell : ICell
{
    public float Width { get; }
    public bool IsRefreshable => false;
    public string? Text { get; }

    public TextCell(string? text)
    {
        Text = text;
        Width = text == null ? 0f : Verse.Text.CalcSize(text).x;
    }

    public void Draw(Rect rect)
    {
        if (Text != null)
        {
            rect.Label(Text, GUIStyles.TableCell.String);
        }
    }
}
