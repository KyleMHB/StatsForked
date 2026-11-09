using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using RimWorldDevTests;
using Stats;
using Verse;

namespace StatsForked.DevTests;

// Expected behaviour (README "Features"/"Usage"/"Configuration"): filters narrow the displayed defs; a saved table preset
// stores the active filters and visible columns in the mod settings and applying it restores them; a preset marked default is
// applied when the table opens; deleting a preset removes it from the settings.
// Mechanical setup: the Apparel table is opened from the Open-table menu and drawn. The Label column's text filter is set
// through its preset state (the same call a preset uses), and the toolbar's private preset actions (save, apply, set default,
// delete) and the column/filter reset are invoked by reflection; the filter window, toolbar and menus are not clicked. Changes
// are queued intents, so each step draws the table before it is measured.
public class PresetsBatch : Batch
{
    private const int Frames = 15;
    private const string PresetName = "DevTests duster", FilterText = "duster";
    private static readonly FieldInfo Tables = AccessTools.Field(typeof(Stats.MainTabWindow), "_tables");
    private static readonly FieldInfo Menu = AccessTools.Field(typeof(Stats.MainTabWindow), "_tableDefsMenu");
    private static readonly FieldInfo MenuOptions = AccessTools.Field(typeof(FloatMenu), "options");

    public override string Id => "stats-presets";
    public override int SegmentCount => 1;
    public override int MapSize => 75;
    public override TimeSpan WallLimit => TimeSpan.FromMinutes(5);
    public override IReadOnlyList<PlannedCase> Plan { get; } = new[]
    {
        "stats.filter", "stats.preset-save", "stats.preset-apply", "stats.preset-default", "stats.preset-delete",
    }.Select(id => new PlannedCase(id, Levels.Mechanical)).ToArray();

    private static MainButtonDef Button => DefDatabase<MainButtonDef>.GetNamedSilentFail("StatsTab");
    private static Stats.MainTabWindow OpenWindow => Find.WindowStack.WindowOfType<Stats.MainTabWindow>();

    // The ObjectTable<T> behind one open table, with its private members.
    private sealed class Table
    {
        private readonly object widget;
        private readonly Type type;
        public Table(object widget) { this.widget = widget; type = widget.GetType(); }
        private object Call(string name, params object[] args) => AccessTools.Method(type, name).Invoke(widget, args);
        public int Rows
        {
            get
            {
                object session = AccessTools.Field(type, "_tableSession").GetValue(widget);
                object frame = AccessTools.Property(session.GetType(), "Frame").GetValue(session);
                return ((ICollection)AccessTools.Property(frame.GetType(), "Rows").GetValue(frame)).Count;
            }
        }
        public List<string> Columns => (List<string>)Call("CaptureVisibleColumnDefNames");
        public void SetColumns(List<string> names) => Call("ApplyVisibleColumns", names);
        public void ResetFilters() => Call("ResetFilters");
        public void SavePreset(string name) => Call("SavePreset", name);
        public void ApplyPreset(TablePreset p) => Call("ApplyPreset", p);
        public void SetDefault(TablePreset p) => Call("SetDefaultPreset", p);
        public void Delete(TablePreset p) => Call("DeletePreset", p);

        // The first text filter, which belongs to the Label column; the preset state uses the "contains" operator.
        public (string column, Stats.Filters.IPresettableFilter filter) LabelFilter()
        {
            foreach (var entry in (IEnumerable)AccessTools.Field(type, "_filters").GetValue(widget))
            {
                var w = AccessTools.Property(entry.GetType(), "Widget")?.GetValue(entry) ?? AccessTools.Field(entry.GetType(), "Widget")?.GetValue(entry);
                if (w is Stats.Filters.StringFilter s)
                {
                    var key = AccessTools.Property(entry.GetType(), "Key")?.GetValue(entry) ?? AccessTools.Field(entry.GetType(), "Key")?.GetValue(entry);
                    return ((string)key, s);
                }
            }
            return (null, null);
        }
    }

    private static TablePreset Saved(string table) =>
        StatsMod.Instance.Settings.presets.FirstOrDefault(p => p.tableDefName == table && p.name == PresetName);

    public override IEnumerable<Wait> Segment(int index, BatchContext ctx)
    {
        var settings = StatsMod.Instance.Settings;
        settings.openTableDefNames.Clear();
        settings.openTableNames.Clear();
        settings.activeTableIndex = -1;
        settings.presets.RemoveAll(p => p.name == PresetName);
        ctx.Setup("Developer cleared the Stats table session and any earlier test preset in the runner's isolated mod settings");

        Find.MainTabsRoot.SetCurrentTab(Button, false);
        yield return Wait.Frames(30);
        var window = OpenWindow;
        var options = window == null ? new List<FloatMenuOption>() : (List<FloatMenuOption>)MenuOptions.GetValue(Menu.GetValue(window));
        var apparelDef = DefDatabase<TableDef>.AllDefsListForReading.FirstOrDefault(d => d.defName.IndexOf("Apparel", StringComparison.OrdinalIgnoreCase) >= 0);
        var option = apparelDef == null ? null : options.FirstOrDefault(o => o.Label == apparelDef.LabelCap.ToString());
        if (!Kit.Require(ctx, "stats.filter", (window != null, "the Stats tab opened"), (option != null, $"the Open-table menu lists the Apparel table ({apparelDef?.defName ?? "no def"})")))
        {
            Kit.SkipRemaining(ctx, this, "the Apparel table could not be opened", CoverageReasons.SetupFailed);
            yield break;
        }
        option.action();
        yield return Wait.Frames(Frames);
        var record = ((IList)Tables.GetValue(window)).Cast<object>().Last();
        var table = new Table(AccessTools.Field(record.GetType(), "TableWidget").GetValue(record));
        string tableName = apparelDef.defName;
        int fullRows = table.Rows;
        var fullColumns = table.Columns;
        var (labelColumn, labelFilter) = table.LabelFilter();
        ctx.Setup($"Developer opened table {tableName}: {fullRows} rows, {fullColumns.Count} visible columns; text filter on {labelColumn ?? "none"}");
        if (!Kit.Require(ctx, "stats.filter", (labelFilter != null, "the table has a text filter"), (fullRows > 1, "the table shows more than one row")))
        {
            Kit.SkipRemaining(ctx, this, "no text filter or too few rows", CoverageReasons.SetupFailed);
            yield break;
        }

        // Filter: "contains duster" on the Label column.
        int expectedRows = DefDatabase<ThingDef>.AllDefsListForReading.Count(d => d.IsApparel && d.label != null && d.label.IndexOf(FilterText, StringComparison.CurrentCultureIgnoreCase) >= 0);
        labelFilter.DeserializeState("~=|" + FilterText);
        yield return Wait.Frames(Frames);
        int filteredRows = table.Rows;
        ctx.Begin("stats.filter").Expect("rows", $"1..{fullRows - 1}")            .Observe("rows", filteredRows).Observe("fullRows", fullRows).Observe("apparelDefsMatching", expectedRows)
            .Check(filteredRows > 0 && filteredRows < fullRows, $"the Label filter \"contains {FilterText}\" did not narrow the table");

        // Save: the preset records the filter and the visible columns, and the settings file on disk holds it.
        table.SavePreset(PresetName);
        var preset = Saved(tableName);
        string file = Directory.GetFiles(GenFilePaths.ConfigFolderPath, "Mod_*Stats*.xml").FirstOrDefault();
        bool onDisk = file != null && File.ReadAllText(file).Contains(PresetName);
        string filterState = preset?.filterStates.FirstOrDefault()?.state ?? "none";
        bool saveOk = preset != null && preset.filterStates.Count == 1 && filterState == "~=|" + FilterText
            && preset.visibleColumnDefNames.SequenceEqual(fullColumns) && onDisk;
        ctx.Begin("stats.preset-save").Expect("filterStates", 1).Expect("filterState", "~=|" + FilterText).Expect("visibleColumns", fullColumns.Count)
            .Expect("inSettingsFile", true)
            .Observe("filterStates", preset?.filterStates.Count ?? 0).Observe("filterState", filterState)
            .Observe("visibleColumns", preset?.visibleColumnDefNames.Count ?? 0).Observe("inSettingsFile", onDisk).Observe("settingsFile", Path.GetFileName(file ?? "none"))
            .Check(saveOk, "the saved preset is missing, did not record the filter or the visible columns, or was not written to the settings file");
        if (preset == null) { Kit.SkipRemaining(ctx, this, "no preset was saved", CoverageReasons.SetupFailed); yield break; }

        // Apply: from an unfiltered table with one column hidden, the preset restores both.
        var fewer = fullColumns.Take(fullColumns.Count - 1).ToList();
        table.ResetFilters();
        table.SetColumns(fewer);
        yield return Wait.Frames(Frames);
        if (Kit.Require(ctx, "stats.preset-apply", (table.Rows == fullRows, $"the reset table shows all {fullRows} rows (shows {table.Rows})"),
                (table.Columns.Count == fewer.Count, $"one column is hidden ({table.Columns.Count} visible)")))
        {
            ctx.Setup($"Developer reset the filters and hid column {fullColumns.Last()}");
            table.ApplyPreset(preset);
            yield return Wait.Frames(Frames);
            ctx.Begin("stats.preset-apply").Expect("rows", filteredRows).Expect("visibleColumns", fullColumns.Count)
                .Observe("rows", table.Rows).Observe("visibleColumns", table.Columns.Count)
                .Check(table.Rows == filteredRows && table.Columns.SequenceEqual(fullColumns), "applying the preset did not restore its filter and visible columns");
        }

        // Default: a preset marked default is applied when the table is opened by a new window.
        table.SetDefault(preset);
        table.ResetFilters();
        yield return Wait.Frames(Frames);
        bool reset = table.Rows == fullRows;
        Find.MainTabsRoot.EscapeCurrentTab(false);
        yield return Wait.Frames(5);
        // Each TableRecord builds a new table widget and applies the table's default preset to it.
        var restoredWindow = new Stats.MainTabWindow();
        var restoredRecords = ((IList)Tables.GetValue(restoredWindow)).Cast<object>().ToList();
        var restoredRecord = restoredRecords.FirstOrDefault(r => (string)AccessTools.Property(r.GetType(), "DefName").GetValue(r) == tableName);
        ctx.Setup($"Developer marked '{PresetName}' default, reset the filters (all rows shown {reset}), closed the tab and showed a new Stats window ({restoredRecords.Count} tables restored)");
        restoredWindow.def = Button; // MainTabWindow.PostOpen reads its main button def
        Find.WindowStack.Add(restoredWindow);
        yield return Wait.Frames(Frames);
        var restored = restoredRecord == null ? null : new Table(AccessTools.Field(restoredRecord.GetType(), "TableWidget").GetValue(restoredRecord));
        int restoredRows = restored?.Rows ?? -1;
        ctx.Begin("stats.preset-default").Expect("isDefault", true).Expect("rows", filteredRows)
            .Observe("isDefault", preset.isDefault).Observe("rows", restoredRows).Observe("restoredTables", restoredRecords.Count)
            .Check(reset && preset.isDefault && restoredRows == filteredRows, "the default preset was not applied when the table was opened");
        restoredWindow.Close(false);
        yield return Wait.Frames(5);

        // Delete: the preset leaves the settings and the settings file.
        table.Delete(preset);
        bool stillOnDisk = file != null && File.ReadAllText(file).Contains(PresetName);
        ctx.Begin("stats.preset-delete").Expect("presetInSettings", false).Expect("inSettingsFile", false)
            .Observe("presetInSettings", Saved(tableName) != null).Observe("inSettingsFile", stillOnDisk)
            .Check(Saved(tableName) == null && !stillOnDisk, "the deleted preset is still in the settings or the settings file");
    }
}
