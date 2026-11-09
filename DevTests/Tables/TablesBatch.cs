using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorldDevTests;
using Stats;
using Verse;

namespace StatsForked.DevTests;

// Expected behaviour (README; TESTING.md): the Stats main tab opens every table listed in its Open-table menu and draws it
// without errors; the open tables are remembered in the mod settings and restored by a new window; tables still draw after a
// save and reload. Any error the game logs while a case is open (including "Exception filling window") is reported against it.
// Mechanical setup: the tab is opened through MainTabsRoot and each Open-table menu option's action is invoked directly; the
// menu, table list and toolbar are not clicked. Filters, presets and sorting are not exercised (TESTING.md's table-frame
// regression is covered by Stats.Tests, not here).
public class TablesBatch : Batch
{
    private const int FramesPerTable = 15;
    private static readonly FieldInfo Tables = AccessTools.Field(typeof(Stats.MainTabWindow), "_tables");
    private static readonly FieldInfo Menu = AccessTools.Field(typeof(Stats.MainTabWindow), "_tableDefsMenu");
    private static readonly FieldInfo MenuOptions = AccessTools.Field(typeof(FloatMenu), "options");
    private static readonly MethodInfo SetActive = AccessTools.Method(typeof(Stats.MainTabWindow), "SetActiveTable");

    public override string Id => "stats-tables";
    public override int SegmentCount => 2;
    public override int MapSize => 75;
    public override TimeSpan WallLimit => TimeSpan.FromMinutes(10);
    public override IReadOnlyList<PlannedCase> Plan { get; } = new[]
    {
        "stats.loaded", "stats.open-window", "stats.draw-tables", "stats.session-restore", "stats.reload-draw",
    }.Select(id => new PlannedCase(id, Levels.Mechanical)).ToArray();

    private static MainButtonDef Button => DefDatabase<MainButtonDef>.GetNamedSilentFail("StatsTab");
    private static Stats.MainTabWindow OpenWindow => Find.WindowStack.WindowOfType<Stats.MainTabWindow>();
    private static IList OpenTables(Stats.MainTabWindow window) => (IList)Tables.GetValue(window);

    // A table's session publishes a frame only from ObjectTable.Draw (PrepareFrame); the empty frame has revision 0.
    // So revision > 0 proves the table was drawn, and the frame's row count is what it showed.
    private static (string name, long revision, int rows) FrameOf(object tableRecord)
    {
        object widget = AccessTools.Field(tableRecord.GetType(), "TableWidget").GetValue(tableRecord);
        object session = AccessTools.Field(widget.GetType(), "_tableSession").GetValue(widget);
        object frame = AccessTools.Property(session.GetType(), "Frame").GetValue(session);
        long revision = (long)AccessTools.Property(frame.GetType(), "Revision").GetValue(frame);
        int rows = ((ICollection)AccessTools.Property(frame.GetType(), "Rows").GetValue(frame)).Count;
        string name = (string)AccessTools.Property(tableRecord.GetType(), "DefName").GetValue(tableRecord);
        return (name, revision, rows);
    }

    private static List<(string name, long revision, int rows)> Frames(Stats.MainTabWindow window) =>
        OpenTables(window).Cast<object>().Select(FrameOf).ToList();

    public override IEnumerable<Wait> Segment(int index, BatchContext ctx)
    {
        if (index == 1)
        {
            foreach (var w in Reloaded(ctx)) yield return w;
            yield break;
        }

        var defs = DefDatabase<TableDef>.AllDefsListForReading;
        var badWorkers = new List<string>();
        foreach (var def in defs)
        {
            try { if (def.Worker == null) badWorkers.Add(def.defName); }
            catch (Exception e) { badWorkers.Add($"{def.defName} ({e.GetType().Name})"); }
        }
        ctx.Begin("stats.loaded").Expect("tabWindowClass", typeof(Stats.MainTabWindow).FullName).Expect("tableDefs", "> 0").Expect("badWorkers", "none")
            .Observe("tabWindowClass", Button?.tabWindowClass?.FullName ?? "missing").Observe("tableDefs", defs.Count)
            .Observe("badWorkers", badWorkers.Count == 0 ? "none" : string.Join(", ", badWorkers))
            .Observe("activeMods", ModsConfig.ActiveModsInLoadOrder.Count())
            .Check(Button?.tabWindowClass == typeof(Stats.MainTabWindow) && defs.Count > 0 && badWorkers.Count == 0,
                "the Stats main button, its window class or a table worker is missing");

        // Start from an empty table session. The runner's mod settings live in its own save-data folder.
        var settings = StatsMod.Instance.Settings;
        settings.openTableDefNames.Clear();
        settings.openTableNames.Clear();
        settings.activeTableIndex = -1;
        ctx.Setup("Developer cleared the Stats table session in the runner's isolated mod settings");

        Find.MainTabsRoot.SetCurrentTab(Button, false);
        yield return Wait.Frames(30);
        var window = OpenWindow;
        ctx.Begin("stats.open-window").Expect("windowOpen", true).Expect("openTables", 0)
            .Observe("windowOpen", window != null).Observe("openTables", window == null ? -1 : OpenTables(window).Count)
            .Check(window != null, "the Stats tab did not open its window");
        if (window == null) yield break;

        // Open every table from the window's own Open-table menu; each becomes active and draws for a few frames.
        var options = (List<FloatMenuOption>)MenuOptions.GetValue(Menu.GetValue(window));
        var draw = ctx.Begin("stats.draw-tables").Expect("tablesOpened", options.Count).Expect("windowOpen", true);
        var labels = new List<string>();
        foreach (var option in options)
        {
            labels.Add(option.Label);
            option.action();
            yield return Wait.Frames(FramesPerTable);
        }
        int opened = OpenTables(window).Count;
        var frames = Frames(window);
        var undrawn = frames.Where(f => f.revision <= 0).Select(f => f.name).ToList();
        draw.Expect("undrawnTables", "none")
            .Observe("tablesOpened", opened).Observe("windowOpen", OpenWindow == window).Observe("framesPerTable", FramesPerTable)
            .Observe("undrawnTables", undrawn.Count == 0 ? "none" : string.Join(", ", undrawn))
            .Observe("rowsShown", string.Join(", ", frames.Select(f => $"{f.name}={f.rows}")))
            .Check(opened == options.Count && OpenWindow == window && undrawn.Count == 0,
                "a table did not open or was never drawn, or the window closed while drawing tables");

        // Closing saves the session; a new window restores it from the settings.
        Find.MainTabsRoot.EscapeCurrentTab(false);
        yield return Wait.Frames(5);
        int saved = settings.openTableDefNames.Count;
        int restored = OpenTables(new Stats.MainTabWindow()).Count;
        ctx.Begin("stats.session-restore").Expect("savedTables", opened).Expect("restoredTables", opened)
            .Observe("savedTables", saved).Observe("restoredTables", restored).Observe("activeTableIndex", settings.activeTableIndex)
            .Check(saved == opened && restored == opened, "closing the tab did not save the open tables, or a new window did not restore them");

        ctx.Remember("opened", opened.ToString());
        yield return Wait.Reload();
    }

    private IEnumerable<Wait> Reloaded(BatchContext ctx)
    {
        int expected = int.Parse(ctx.Recall("opened"));
        Find.MainTabsRoot.SetCurrentTab(Button, false);
        yield return Wait.Frames(30);
        var window = OpenWindow;
        var scope = ctx.Begin("stats.reload-draw").Expect("windowOpen", true).Expect("openTables", expected).Expect("tablesRedrawn", expected);
        if (window == null)
        {
            scope.Observe("windowOpen", false).Check(false, "the Stats tab did not open after reload");
            yield break;
        }
        // Revisions only advance when a table's layout changes, so each table is compared with its own value before it is shown.
        var tables = OpenTables(window).Cast<object>().ToList();
        var notRedrawn = new List<string>();
        foreach (var table in tables)
        {
            var before = FrameOf(table);
            SetActive.Invoke(window, new[] { table });
            yield return Wait.Frames(5);
            var after = FrameOf(table);
            if (after.revision <= 0 || (before.revision > 0 && after.rows == 0 && before.rows > 0)) notRedrawn.Add(after.name);
        }
        Find.MainTabsRoot.EscapeCurrentTab(false);
        int redrawn = tables.Count - notRedrawn.Count;
        scope.Observe("windowOpen", true).Observe("openTables", tables.Count).Observe("tablesRedrawn", redrawn)
            .Observe("notRedrawn", notRedrawn.Count == 0 ? "none" : string.Join(", ", notRedrawn))
            .Check(tables.Count == expected && redrawn == expected, "after reload the tab did not hold or redraw every open table");
    }
}
