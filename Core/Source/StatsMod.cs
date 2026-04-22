using RimWorld;
using Verse;

namespace Stats;

public sealed class StatsMod : Mod
{
    public static StatsMod Instance { get; private set; } = null!;
    public StatsSettings Settings { get; }

    public StatsMod(ModContentPack content) : base(content)
    {
        Instance = this;
        Settings = GetSettings<StatsSettings>();
    }

    public override string SettingsCategory()
    {
        return "Stats";
    }
}
