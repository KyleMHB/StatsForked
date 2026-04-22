using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.TableWorkers;
using Stats.Utils;
using Verse;

namespace Stats.Bionics;

public sealed class BionicLabelColumnWorker(ColumnDef columnDef) : ColumnWorker<BionicOperation, BionicLabelColumnWorker.Cell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override Cell MakeCell(BionicOperation @object)
    {
        return new Cell(@object.Recipe);
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
        public string? Text { get; }

        public Cell(RecipeDef recipe)
        {
            Text = recipe.LabelCap.RawText;
            Width = Verse.Text.CalcSize(Text).x;
        }

        public void Draw(UnityEngine.Rect rect)
        {
            if (Text != null)
            {
                rect.Label(Text, GUIStyles.TableCell.String);
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

        public void Draw(UnityEngine.Rect rect)
        {
            if (Text != null)
            {
                rect.Label(Text, GUIStyles.TableCell.String);
            }
        }
    }
}
