using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats;

public sealed class StatsSettings : ModSettings
{
    public List<TablePreset> presets = [];
    public List<string> openTableDefNames = [];
    public int activeTableIndex = -1;
    public bool expandedMultiValueCells;

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref presets, nameof(presets), LookMode.Deep);
        Scribe_Collections.Look(ref openTableDefNames, nameof(openTableDefNames), LookMode.Value);
        Scribe_Values.Look(ref activeTableIndex, nameof(activeTableIndex), -1);
        Scribe_Values.Look(ref expandedMultiValueCells, nameof(expandedMultiValueCells));

        if (Scribe.mode == LoadSaveMode.PostLoadInit)
        {
            presets = presets?
                .Where(preset => preset != null)
                .ToList() ?? [];

            openTableDefNames = openTableDefNames?
                .Where(defName => defName != null)
                .ToList() ?? [];
            if (openTableDefNames.Count == 0)
            {
                activeTableIndex = -1;
            }
            else
            {
                activeTableIndex = activeTableIndex < 0
                    ? 0
                    : activeTableIndex >= openTableDefNames.Count
                        ? openTableDefNames.Count - 1
                        : activeTableIndex;
            }

            foreach (TablePreset preset in presets)
            {
                preset.Normalize();
            }

            HashSet<string> defaultTables = [];
            foreach (TablePreset preset in presets)
            {
                if (preset.isDefault && defaultTables.Add(preset.tableDefName) == false)
                {
                    preset.isDefault = false;
                }
            }
        }
    }
}

public sealed class TablePreset : IExposable
{
    public string tableDefName = "";
    public string name = "";
    public bool isDefault;
    public bool showVariants;
    public bool expandMultiValueCells;
    public List<string> visibleColumnDefNames = [];
    public List<FilterPresetState> filterStates = [];

    public void ExposeData()
    {
        Scribe_Values.Look(ref tableDefName, nameof(tableDefName), "");
        Scribe_Values.Look(ref name, nameof(name), "");
        Scribe_Values.Look(ref isDefault, nameof(isDefault));
        Scribe_Values.Look(ref showVariants, nameof(showVariants));
        Scribe_Values.Look(ref expandMultiValueCells, nameof(expandMultiValueCells));
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
    public string filterId = "";
    public string label = "";
    public string state = "";

    public void ExposeData()
    {
        Scribe_Values.Look(ref columnDefName, nameof(columnDefName), "");
        Scribe_Values.Look(ref filterId, nameof(filterId), "");
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
        filterId ??= "";
        label ??= "";
        state ??= "";
    }
}
