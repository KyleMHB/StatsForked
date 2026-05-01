using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public sealed class StatsSettings : ModSettings
{
    public List<TablePreset> presets = [];

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref presets, nameof(presets), LookMode.Deep);

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            presets = presets?
                .Where(preset => preset != null)
                .ToList() ?? [];

            foreach (TablePreset preset in presets)
            {
                preset.Normalize();
            }
        }
    }
}

public sealed class TablePreset : IExposable
{
    public string tableDefName = "";
    public string name = "";
    public bool showVariants;
    public List<string> visibleColumnDefNames = [];
    public List<FilterPresetState> filterStates = [];

    public void ExposeData()
    {
        Scribe_Values.Look(ref tableDefName, nameof(tableDefName), "");
        Scribe_Values.Look(ref name, nameof(name), "");
        Scribe_Values.Look(ref showVariants, nameof(showVariants));
        Scribe_Collections.Look(ref visibleColumnDefNames, nameof(visibleColumnDefNames), LookMode.Value);
        Scribe_Collections.Look(ref filterStates, nameof(filterStates), LookMode.Deep);

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            Normalize();
        }
    }

    public void Normalize()
    {
        tableDefName ??= "";
        name ??= "";
        visibleColumnDefNames = visibleColumnDefNames?
            .Where(defName => defName != null)
            .ToList() ?? [];
        filterStates = filterStates?
            .Where(state => state != null)
            .ToList() ?? [];

        foreach (FilterPresetState filterState in filterStates)
        {
            filterState.Normalize();
        }
    }
}

public sealed class FilterPresetState : IExposable
{
    public string columnDefName = "";
    public string label = "";
    public string state = "";

    public void ExposeData()
    {
        Scribe_Values.Look(ref columnDefName, nameof(columnDefName), "");
        Scribe_Values.Look(ref label, nameof(label), "");
        Scribe_Values.Look(ref state, nameof(state), "");

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            Normalize();
        }
    }

    public void Normalize()
    {
        columnDefName ??= "";
        label ??= "";
        state ??= "";
    }
}
