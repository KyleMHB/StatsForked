using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using Stats.Utils;
using UnityEngine;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class Apparel_ValidLifeStageColumnWorker(ColumnDef columnDef) : ColumnWorker<DefBasedObject, Apparel_ValidLifeStageColumnWorker.LifeStageCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.String;

    protected override LifeStageCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is ThingDef thingDef)
        {
            HashSet<DevelopmentalStage> validLifeStages = GetValidLifeStages(thingDef);
            if (validLifeStages.Count > 0)
            {
                return new LifeStageCell(validLifeStages);
            }
        }

        return default;
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<DevelopmentalStage>> valueFieldFilterOptions = ((IRefRecordsProvider<ThingDef>)tableWorker).Records
            .SelectMany(GetValidLifeStages)
            .Distinct()
            .OrderBy(lifeStage => lifeStage)
            .Select(lifeStage => new NTMFilterOption<DevelopmentalStage>(lifeStage, GetLifeStageString(lifeStage)));
        Filter valueFieldFilter = new MTMFilter<DevelopmentalStage>((int row) => this[row].Value ?? [], valueFieldFilterOptions);
        int CompareByCellText(int row1, int row2) => Comparer<string?>.Default.Compare(this[row1].Text, this[row2].Text);
        CellField valueField = new(Def.TitleWidget, valueFieldFilter, CompareByCellText);

        return [valueField];
    }

    private static HashSet<DevelopmentalStage> GetValidLifeStages(ThingDef thingDef)
    {
        HashSet<DevelopmentalStage> validLifeStages = [];

        DevelopmentalStage developmentalStageFilter = thingDef.apparel?.developmentalStageFilter ?? DevelopmentalStage.None;
        foreach (DevelopmentalStage developmentStage in System.Enum.GetValues(typeof(DevelopmentalStage)))
        {
            int rawValue = (int)developmentStage;
            if
            (
                rawValue != 0
                && (rawValue & (rawValue - 1)) == 0
                && developmentalStageFilter.HasFlag(developmentStage)
            )
            {
                validLifeStages.Add(developmentStage);
            }
        }

        return validLifeStages;
    }

    private static string GetLifeStageString(DevelopmentalStage lifeStage)
    {
        return lifeStage.ToString().Translate().CapitalizeFirst().RawText;
    }

    public readonly struct LifeStageCell : ICell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public IReadOnlyCollection<DevelopmentalStage>? Value { get; }
        public string? Text { get; }

        public LifeStageCell(IReadOnlyCollection<DevelopmentalStage> value)
        {
            Value = value;
            if (value.Count > 0)
            {
                Text = string.Join("\n", value.OrderBy(lifeStage => lifeStage).Select(GetLifeStageString));
                Width = Text.CalcSize(GUIStyles.TableCell.String).x;
            }
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
