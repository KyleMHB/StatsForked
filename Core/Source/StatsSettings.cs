using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class StatsSettings : ModSettings
{
    public List<TablePreset> presets = [];

    public override void ExposeData()
    {
        Scribe_Collections.Look(ref presets, nameof(presets), LookMode.Deep);
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
    }
}
