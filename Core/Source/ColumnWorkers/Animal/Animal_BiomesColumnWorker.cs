using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using RimWorld;
using Stats.Widgets;
using UnityEngine;
using Verse;

namespace Stats;

public sealed class Animal_BiomesColumnWorker : ColumnWorker<ThingAlike>
{
    public Animal_BiomesColumnWorker(ColumnDef columnDef) : base(columnDef, ColumnCellStyle.String)
    {
    }
    // This exists mainly for consistency, for when this column worker is used by several tables.
    private static readonly Func<ThingAlike, List<BiomeRecord>> GetBiomeRecords =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var biomeRecords = new List<BiomeRecord>();
        var raceProps = thing.Def.race;

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
    private static readonly Func<ThingAlike, HashSet<BiomeDef>> GetBiomeDefs =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var biomeRecords = GetBiomeRecords(thing);

        return biomeRecords.Select(biomeRecord => biomeRecord.BiomeDef).ToHashSet();
    });
    private static readonly Func<ThingAlike, float> GetAverageAnimalCommonality =
    FunctionExtensions.Memoized((ThingAlike thing) =>
    {
        var biomeRecords = GetBiomeRecords(thing);

        if (biomeRecords.Count == 0)
        {
            return 0f;
        }

        var animalCommonalitySum = 0f;

        foreach (var biomeRecord in biomeRecords)
        {
            animalCommonalitySum += biomeRecord.Commonality;
        }

        return animalCommonalitySum / biomeRecords.Count;
    });
    public override Widget? GetTableCellWidget(ThingAlike thing)
    {
        var biomeRecords = GetBiomeRecords(thing);

        if (biomeRecords.Count > 0)
        {
            var text = biomeRecords[0].ToString();
            string? tooltip = null;

            if (biomeRecords.Count > 1)
            {
                text += $" ({biomeRecords.Count})".Colorize(Color.grey);

                var stringBuilder = new StringBuilder();

                foreach (var biomeRecord in biomeRecords)
                {
                    stringBuilder.AppendLine(biomeRecord.ToString());
                }

                tooltip = stringBuilder.ToString();
            }

            Widget widget = new Label(text);

            if (tooltip != null)
            {
                widget = widget.Tooltip(tooltip);
            }

            return widget;
        }

        return null;
    }
    public override FilterWidget<ThingAlike> GetFilterWidget(IEnumerable<ThingAlike> tableRecords)
    {
        return Make.MTMDefFilter(GetBiomeDefs, tableRecords);
    }
    public override int Compare(ThingAlike thing1, ThingAlike thing2)
    {
        return GetAverageAnimalCommonality(thing1).CompareTo(GetAverageAnimalCommonality(thing2));
    }

    private readonly record struct BiomeRecord(BiomeDef BiomeDef, float Commonality)
    {
        public override string ToString()
        {
            return $"{BiomeDef.LabelCap}: {Commonality.ToString("0.###").Colorize(Globals.GUI.TextHighlightColor)}";
        }
    }
}
