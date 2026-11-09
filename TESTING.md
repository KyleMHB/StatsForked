# Testing and acceptance

## Automated checks

Run the full test and build matrix from the repository root:

```powershell
dotnet test Stats.sln -c Debug -m:1 /p:UseSharedCompilation=false
dotnet build Stats.sln -c Debug -m:1 /p:UseSharedCompilation=false
dotnet build Stats.sln -c Release -m:1 /p:UseSharedCompilation=false
```

The `Stats.Tests` project targets `net48` and sets `EnableRuntimePackaging=false`. Its table tests use `TableSession<TObject>` and its Odyssey tests use `IOdysseyProjection`; private registry and drawing helpers are not tested directly. The XML worker-resolution test scans the mod XML under `Core`, `Biotech`, `Anomaly`, `CE`, and `Odyssey`, including patch XML, and resolves each non-comment `workerClass` element against the built project assemblies.

## Table-frame regression

The original failure was an `ArgumentOutOfRangeException` in `ObjectTable.GetRowHeight` while drawing visible rows. Unity's immediate-mode UI can run a Layout pass, apply a filter or preset, and then draw after the row list has changed. The positional `_rowHeights` array still described the previous row generation, so drawing paired a new row index with old geometry.

`TableSession<TObject>` queues GUI mutations and prepares one immutable `TableFrame<TObject>` for the next frame. The frame copies row order, row heights, visible columns, widths, pinned counts, aggregate geometry, and a monotonic revision together. Its required invariant is `Rows.Count == RowHeights.Count`; drawing consumes only that captured frame. A failed preparation keeps the previous frame, retains the queued intent for a compatible retry, and records a diagnostic.

Automated coverage also checks tab-name normalization, migration from settings without names, duplicate-tab name alignment, and configured reloadable ammo properties. UI interaction and settings persistence across a game restart remain manual acceptance checks.

Automated coverage includes row growth and shrinkage, variable expanded heights, pinned and empty tables, scrolling past the last row, deferred/coalesced actions, atomic configuration rollback, worker lifecycle disposal, and Odyssey reflection fallback/exception containment.

## RimWorld acceptance scenarios

With RimWorld 1.6 and 1.7 and the installed official DLC enabled, exercise the following in a representative save:

- Shrink and reset filters while the table is scrolled.
- Toggle expanded values, variants, and quality.
- Apply presets that add rows or hidden filters.
- Pin, reorder, and resize rows and columns.
- Remove and reopen tables.
- Open two tabs of the same type, give them different names, and confirm their filters remain unchanged. Close/reopen Stats and restart the game; names must retain their tab associations. Remove a tab and verify surviving names, clear a name to restore the original label, and load settings created before tab names existed.
- With Yayo's Combat 3 (Continued) enabled, compare Ammos and Reload cost per ammo against a weapon's info card, including custom ammo and cost overrides. Sort/filter both columns and check ammo icon tooltips. Repeat with the ammo system disabled, then without Yayo; Stats must load without missing-def errors.
- Complete research while a research-status filter is active.

When CE is installed locally, repeat the relevant weapon and apparel tables with CE enabled. After each scenario, inspect the newest usable RimWorld log. Acceptance requires no Stats exceptions, stale-frame diagnostics, disposed-worker callbacks, or secondary word-wrap warning. The RimWorld UI scenarios remain manual because the test project does not host Unity's immediate-mode GUI or RimWorld's live Def database.

## Table controls report, 2026-10-19

The regression suite exercises the production sequence of a rejected application followed by frame preparation. Both thrown exceptions and explicit rejection preserve the previous frame and configuration, retain diagnostics, and retry the pending changes successfully. RPM tests invoke the actual worker with zero and negative burst intervals and require an empty cell without an exception.

Observed with cached RimWorld 1.6 runtime assemblies: the four new regression cases failed before the fixes and passed afterward. The Release suite passed all 61 cases. These checks do not reproduce Unity mouse input or establish that Red Grail's full controls issue is resolved.

For an in-game reproduction on RimWorld 1.6.4871, use the reporter's mod set and save. Click variants, expanded view, quality, column visibility, filters, and sorting. Check Player.log for `Stats table` warnings, which now include the table identifier and rejected transition details. Repeat the same actions with Harmony and Stats only, then save and reload. Record the active mods and whether resizing and creating tabs still work. In-game verification remains pending; the installed game is 1.7.
## Quality validation regression, 2026-10-20

Both the 1.6 and 1.7 report logs rejected table changes because Enum.IsDefined received a boxed Int32 for the byte-backed QualityCategory enum. Table updates now reject integers outside the byte range before validating a typed QualityCategory value. Unsupported values preserve the current quality.

QualityValidationTests exercises the production validation method with all seven defined qualities and seven invalid saved values, including 256 and 258 to catch byte wraparound. All 14 cases reproduced the reported ArgumentException before the fix. The Release suite passed all 75 tests against the cached RimWorld 1.6 runtime assemblies.

Unity input and live 1.7 acceptance remain manual. Restart RimWorld after updating and exercise sorting, filters, column visibility, variants, expanded view, and quality on weapon, apparel, and other tables. Confirm Player.log has no quality enum type mismatch or rejected-transition warnings. Repeat with the reporter's save and minimal mod set.

## Cached column rows regression, 2026-10-21

Column rebuilds replace worker rows instead of appending to retained caches. ColumnRowRebuildTests exercises staged quality changes through TableSession with the production worker cache, including visible and filter-only roles, repeated quality changes, and row growth, shrinkage, and empty rebuilds. All three cases failed with the previous append behavior and passed after clearing cells before repopulation. All 78 tests passed in Debug and Release against the cached RimWorld 1.6 runtime.

Run the matrix with /p:RimWorldManagedPath=C:\Users\Kyle\AppData\Local\Temp\stats-rimworld-1.6-regression-runtime when the installed game does not provide the targeted runtime. Without actual runtime DLLs, the test output may contain signature-only reference assemblies.

In RimWorld, compare Normal and Legendary values for a quality-sensitive weapon or apparel stat. Repeat with that column hidden and its filter active, change variants, add/remove columns, and restore a saved preset. Confirm values, sorting, and filtering follow the selected quality. Unity UI acceptance remains pending.
## Preset dialog layout, 2026-10-23

The Save preset window is 140 logical pixels tall and assigns Save and Cancel an explicit 30-pixel height. A read-only probe compiles the production dialog and rectangle helpers against captured widget calls, with RimWorld 1.6's title and margin deductions. Before the fix, the 100-pixel window left buttons only 8 pixels high with a 22-pixel text line. After the fix, both buttons are 30 pixels high and fit within the content area.

In game, open Presets > Save current and confirm the button labels are fully visible, Save persists a named preset, and Cancel closes without saving. Repeat windowed and fullscreen. Native Unity rendering remains a manual check.
## In-game developer tests

`DevTests/` uses the shared runner in the sibling `RimWorldDevTests` checkout. The game runs in an isolated save-data folder, so the player's mod list, settings and saves are not touched. `DevTests/devtests.json` defines two mod sets:

- `default`: Harmony, Core and Stats Forked.
- `dlc`: the same plus all five DLCs. Use it with `--mods dlc` to load the Biotech, Anomaly and Odyssey compatibility assemblies.

The DevTests project shadows the root `Directory.Build.props`, so it stays out of `Stats.sln` and the `Runtime Only` packaging. Its runtime folders list only the packaged subfolders (for example `Core/Assemblies`, not `Core`), so `Core/Source` is not compared with the installed mod.

Batch `stats-tables`:

- **`stats.loaded`:** the `StatsTab` main button opens `Stats.MainTabWindow`, and every `TableDef` worker can be created.
- **`stats.open-window`:** the tab opens its window.
- **`stats.draw-tables`:** each Open-table menu option opens its table, which then draws for 15 frames. Each table's `TableSession` frame revision must be above 0, which proves it was drawn, and its row count is recorded.
- **`stats.session-restore`:** closing the tab saves every open table to the settings, and a new window restores them.
- **`stats.reload-draw`:** after a save and reload, every open table is made active and drawn again.

Any error the game logs while a case is open, including "Exception filling window", is reported against that case. The cases call the menu actions and window methods directly; they do not click. Filters, presets, sorting and column edits are not exercised. The table-frame regression above stays covered by `Stats.Tests`.

```powershell
dotnet build DevTests/StatsForked.DevTests.csproj -c Release
python tools/stats_test.py run stats-tables --seed 1
python tools/stats_test.py run stats-tables --mods dlc --seed 1
python tools/stats_test.py close-session
python tools/stats_test.py release-check
```

The installed mod must match the working tree's packaged files, so run `deploy.ps1` first.

### 2026-11-09 run

- **Environment:** RimWorld 1.7.5000 rev1275, seed 1. The working tree was `main` (`0aa47ed`) plus the harness files.
- **Deployment:** the previous 2026-10-21 installation was backed up to `.scratch/devtests-harness/previous-runtime-20261109.zip`, then `deploy.ps1` installed the build. `Stats.sln` has 0 errors and 9 existing warnings: CS0067 unused `OnObjectAdded`/`OnObjectRemoved` events in four table workers, and CS0649 for `ColumnTitleXmlNode.IconElement.Def`.
- **Run `20261109-184656-stats-tables-s1` (`default`, 3 mods):** 5 of 5 passed. All 13 tables were drawn; rows shown included Animals 61, Plants 49 and Apparel 35. 13 tables were saved and restored, and 13 were redrawn after reload.
- **Run `20261109-184718-stats-tables-s1` (`dlc`, 8 mods):** 5 of 5 passed. All 21 tables were drawn, including Genes 232, Animals 115, Apparel 112 and the five Odyssey tables. 21 tables were saved, restored and redrawn.
- **Earlier runs:** `20261109-184512-stats-tables-s1` and `20261109-184542-stats-tables-s1` also passed. They ran before the frame-revision observation was added, so they show only that each table opened.
- `release-check` passed.
- **Not covered:** filters, presets, sorting, column edits, custom table names, the resize control, Combat Extended and Yayo's Combat columns, and the full mod list.
