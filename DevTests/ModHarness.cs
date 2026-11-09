using System;
using System.Collections.Generic;
using RimWorldDevTests;

namespace StatsForked.DevTests;

public sealed class ModHarness : HarnessSettings
{
    public override string DisplayName => "Stats Forked";
    public override string ArgPrefix => "stats";
    public override string PackageId => "kylemhb.statsforked";
    public override Type ProductionType => typeof(global::Stats.StatsMod);
    public override string EvidenceFolder => "StatsForkedTests";

    // Only the packaged subfolders (see Directory.Build.props DeployRuntimeOnly); Core/Source must not count as runtime.
    public override string[] RuntimeFolders => new[]
    {
        "About",
        "Core/Assemblies", "Core/Defs", "Core/Languages", "Core/Patches", "Core/Textures",
        "Biotech/Assemblies", "Biotech/Defs", "Biotech/Patches", "Biotech/Textures",
        "Anomaly/Assemblies", "Anomaly/Defs", "Anomaly/Patches", "Anomaly/Textures",
        "Odyssey/Assemblies", "Odyssey/Defs", "Odyssey/Patches", "Odyssey/Textures",
        "CE/Assemblies", "CE/Defs", "CE/Patches", "CE/Textures",
    };

    public override string[] RuntimeFiles => new[] { "LoadFolders.xml" };
    public override IEnumerable<Batch> CreateBatches() => new Batch[] { new TablesBatch(), new PresetsBatch() };
    public override IEnumerable<string> RunAll => new[] { "stats-tables", "stats-presets" };
}
