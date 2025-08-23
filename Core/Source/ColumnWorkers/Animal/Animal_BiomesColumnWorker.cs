using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Stats.Widgets;
using Verse;

namespace Stats;

public sealed class Animal_BiomesColumnWorker : ColumnWorker<ThingAlike, (HashSet<BiomeDef> Biomes, float AverageCommonality)>
{
    public Animal_BiomesColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    // This exists mainly for consistency, for when this column worker is used by several tables.
    private static readonly Func<ThingDef, List<BiomeRecord>> GetBiomeRecords =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        var biomeRecords = new List<BiomeRecord>();
        var raceProps = thingDef.race;

        if (raceProps?.Animal == true)
        {
            foreach (var biomeDef in DefDatabase<BiomeDef>.AllDefsListForReading)
            {
                var animalCommonality = biomeDef.CommonalityOfAnimal(raceProps.AnyPawnKind);

                if (animalCommonality > 0f)
                {
                    var biomeRecord = new BiomeRecord(biomeDef, animalCommonality);

                    biomeRecords.Add(biomeRecord);
                }
            }
        }

        biomeRecords.SortByDescending(biomeRecord => biomeRecord.Commonality);

        return biomeRecords;
    });
    protected override DataCell GetCell(ThingAlike thing)
    {
        var biomeRecords = GetBiomeRecords(thing.Def);

        if (biomeRecords.Count > 0)
        {
            var text = biomeRecords[0].ToString();
            string? tooltip = null;

            if (biomeRecords.Count > 1)
            {
                text += $" ({biomeRecords.Count})".Colorize(Globals.GUI.TextColorSecondary);

                var stringBuilder = new StringBuilder();

                foreach (var biomeRecord in biomeRecords)
                {
                    stringBuilder.AppendLine(biomeRecord.ToString());
                }

                tooltip = stringBuilder.ToString();
            }

            var animalCommonalitySum = 0f;

            foreach (var biomeRecord in biomeRecords)
            {
                animalCommonalitySum += biomeRecord.Commonality;
            }

            var averageCommonality = animalCommonalitySum / biomeRecords.Count;

            var biomeDefs = biomeRecords.Select(biomeRecord => biomeRecord.BiomeDef).ToHashSet();

            Widget widget = new Label(text).PaddingAbs(ObjectTable.CellPadHor, ObjectTable.CellPadVer);

            if (tooltip != null)
            {
                widget = widget.Tooltip(tooltip);
            }

            return new(widget, (biomeDefs, averageCommonality));
        }

        return new(null, ([], 0f));
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<ThingAlike> tableRecords)
    {
        yield return new(ColumnDef.Title, Make.MTMDefFilter(thing => Cells[thing].Data.Biomes, tableRecords));
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return Cells[thing1].Data.AverageCommonality.CompareTo(Cells[thing2].Data.AverageCommonality);
    }

    private readonly record struct BiomeRecord(BiomeDef BiomeDef, float Commonality)
    {
        public override string ToString()
        {
            return $"{BiomeDef.LabelCap}: {Commonality.ToString("0.###").Colorize(Globals.GUI.TextColorHighlight)}";
        }
    }
}
