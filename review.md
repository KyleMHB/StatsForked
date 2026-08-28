# Stats Forked Mod Review

Review date: 2026-08-28  
Repository: `E:\Coding\Rimworld\Stats Forked`  
Local reviewed revision: `3655cdf`  
Workshop item: `3712317437`  
Package ID: `KyleMHB.StatsForked`  
Game version tested: RimWorld `1.6.4871 rev591`

## Executive verdict

Stats Forked has a useful, generally sound table framework and the recent implementation work compiles, deploys, and starts without mod errors. Runtime testing confirms column resizing and session restoration work. The new expanded Values mode also functions, although its presentation needs refinement.

The current deployed build is not ready for a public Workshop update yet. The toolbar is exposing untranslated `StatsForked_*` keys in English, the hidden-column filter workflow is too difficult to discover or complete, and several older correctness, release, and documentation issues remain. The translation failure should be fixed first because it affects every new toolbar and filter label and likely makes the filter UX substantially harder to understand.

No critical save-corruption or startup-crash defect was found. The highest-risk functional defect outside the new UI is that the inventory tracker currently treats every spawned thing on a relevant map as owned and does not actually test fog visibility.

Recommended release decision: **hold publication, fix AUDIT-014 and AUDIT-015, rerun tests 3, 4, and 6, then publish the seven local commits plus those fixes together.**

## Ratings

| Area | Score | Assessment |
|---|---:|---|
| Correctness | 5/10 | Core tables work, but raw translation keys, ambiguous hidden filtering, and incorrect owned/visible semantics are material defects. |
| Architecture | 7/10 | The table, column worker, filter, preset, and compatibility-module boundaries are sensible. Lifetime ownership is less clear. |
| Maintainability | 6/10 | Naming and organization are mostly understandable, but dead Odyssey assets, label-based persistence, stale packaging rules, and no tests increase change risk. |
| Performance | 6/10 | Normal table rendering is reasonable. Inventory refresh performs full spawned-thing scans and set copies when dirty, with no profiling evidence. |
| Compatibility/currentness | 6/10 | RimWorld 1.6 and all official DLCs loaded cleanly in the tested session. Optional third-party compatibility was not evidenced in the supplied log. Public metadata is inconsistent. |
| Documentation | 4/10 | Local release notes improved, but repository and Workshop identities are wrong in several places and README claims removed content. |
| Test confidence | 5/10 | A real in-game session now covers startup, resize, persistence, and expanded values. Hidden-filter tests are blocked and there is no automated suite. |
| Release quality | 4/10 | Deployment is reproducible and assemblies are uniquely named, but the tested UI is visibly broken and local work is not yet on public `main`. |

## Scope and identities

The reviewed mod is the maintained fork of `AzzkiyOne/Stats`.

- Canonical source repository: `https://github.com/KyleMHB/StatsForked`
- Current Workshop item: `https://steamcommunity.com/sharedfiles/filedetails/?id=3712317437`
- Original Workshop item: `3479566439`
- Supported local game version: RimWorld 1.6 only
- Required dependency: Harmony
- Conditional modules: Biotech, Anomaly, Odyssey, and Combat Extended
- Local repository state at review: seven commits ahead of public `main`, plus the pre-existing untracked `About/PublishedFileId.txt`
- Deployed location tested: `E:\Steam\steamapps\common\RimWorld\Mods\Stats Forked`

The review did not modify mod code or the existing Workshop ID file. This `review.md` is the only review artifact added.

## Architecture map

| Area | Responsibility | Main locations |
|---|---|---|
| Core UI | Main tab, tables, toolbar, drawing, scrolling, sorting, resizing | `Core/Source/MainTabWindow`, `Core/Source/ObjectTable` |
| Table model | Table definitions and generic table construction | `Core/Defs`, `Core/Source/TableWorkers` |
| Columns | Column definitions and per-type value/rendering workers | `Core/Defs`, `Core/Source/ColumnWorkers` |
| Filters | Registered column filters, filter window, filter persistence | `Core/Source/ObjectTable/ObjectTable_Filtering.cs`, `Core/Source/Filters` |
| Presets/settings | Saved columns, filters, defaults, table session state | `Core/Source/ObjectTable/ObjectTable_Presets.cs`, settings classes |
| Inventory state | Owned and visible-on-map caches | `Core/Source/InventoryStateTracker.cs` |
| Compatibility | Conditional definitions and assemblies | `Biotech`, `Anomaly`, `Odyssey`, `CE` |
| Localization | New keyed toolbar/filter strings | `Languages/English/Keyed/StatsForked.xml`, `Core/Source/Localization.cs` |
| Packaging | Version folders, deployment, Publisher Plus exclusions | `LoadFolders.xml`, `deploy.ps1`, `_PublisherPlus.xml` |

## Runtime test results

### Test 1: clean startup and compatibility assembly loading

**Result: Pass for the tested configuration.**

The current `Player.log`, last written at 2026-08-28 11:27:59, shows RimWorld 1.6.4871 loading a save with:

- Harmony
- Core
- Royalty
- Ideology
- Biotech
- Anomaly
- Odyssey
- Stats Forked
- Quality Framework

There is no Stats Forked exception, XML parse failure, missing type, duplicate assembly, or translation parse error. The two Unity fallback-library messages and three dependency-metadata warnings belong to the game or other mods and are unrelated to Stats Forked.

This validates the official-DLC load path and the unique compatibility assembly deployment. It does **not** validate Combat Extended because CE was not active in this log.

### Test 2: column resizing

**Result: Pass, user confirmed.**

No follow-up fix is indicated unless resizing fails at a different UI scale or after restart.

### Test 3: filter a hidden `Can be cultivated` column

**Result: Blocked by UI discoverability.**

The code provides a `Filters` window and an `Add column filter` button, then lists compatible columns. The tester could not find a clear way to add and configure the hidden `Can be cultivated` filter. This is a failed usability outcome even if the underlying filter code is technically reachable.

### Test 4: save and restore a hidden-column filter in a preset

**Result: Blocked by test 3.**

The preset path cannot be meaningfully validated until a hidden filter can be configured. Treat this as untested, not failed persistence.

### Test 5: table session restoration

**Result: Pass, user confirmed.**

The user's later explicit statement that test 5 passes is treated as the result. The earlier grouping of tests 3/4/5 as difficult is interpreted as a description of the filter-menu problem, not a reversal of the later pass result.

### Test 6: expanded multi-value cells

**Result: Partial pass.**

`Values` is the intended control. A red X means compact mode and a green check means expanded mode. The second screenshot confirms that clicking it switches the mode and reveals multi-line content such as the additional Layers, Covers, and Offsets values.

The feature works functionally, but the expanded rows are excessively tall at the tested 1.5× UI scale and the typography/density do not make the extra data easy to scan. The toggle also lacks an immediately clear label such as `Expand values` or `Compact values`.

### Remaining compatibility and regression coverage

The user reports completing the requested testing before this review, but this message does not provide separate observable results for Combat Extended, Fish Traps, Searchable Defs, or a broad table-by-table regression pass. Do not claim those configurations as verified without their logs or explicit results.

## Screenshot findings

### Screenshot 1: compact plant table

- The table itself renders and contains expected plant values.
- The toolbar displays raw keys such as `StatsForked_Filters`, `StatsForked_Columns`, `StatsForked_Values`, `StatsForked_Quality`, and `StatsForked_Presets` instead of English labels.
- The underscores and raw key names create the reported strange top-row appearance. This is primarily a localization-loading failure, not evidence that RimWorld's general font is broken.
- The Values button is off, shown by the red X.

### Screenshot 2: expanded apparel table

- The Values button is on, shown by the green check.
- Expanded values are present and the detailed material tooltip works.
- Rows become very tall, leaving substantial empty space for items with few values.
- The toolbar still displays raw translation keys.
- The test was performed with English selected and RimWorld UI scale set to 1.5.

## Findings

### AUDIT-014: New localization is not loaded at runtime

**Severity: High**  
**Confidence: High**  
**Release blocking: Yes**

Evidence:

- Screenshots show raw `StatsForked_*` keys throughout the toolbar.
- `Languages/English/Keyed/StatsForked.xml:3-63` contains valid-looking English keyed entries.
- `Core/Source/Localization.cs:67-70` sends those keys through RimWorld's `Translate()` API.
- `LoadFolders.xml:3-9` loads `Core` and conditional compatibility roots but does not select a root containing the top-level `Languages` directory.
- The deployed translation file exists, so this is not a missing-copy failure.
- `Player.log` contains no XML or translation parse error.
- Game preferences confirm `English`, so missing non-English coverage is not the explanation.

Most viable fix:

1. Move the keyed file to `Core/Languages/English/Keyed/StatsForked.xml`, so it is under an always-selected load folder.
2. Update deployment and packaging checks to assert that the runtime English keyed file exists under the selected Core load root.
3. Avoid adding the entire mod root to `LoadFolders.xml` unless duplicate loading of `Core`, DLC, and compatibility content has been conclusively ruled out.
4. Add a startup/runtime smoke check for `"StatsForked_Filters".Translate()` returning `Filters`, not the key.
5. Re-run all UI tests after this fix because untranslated controls likely contributed to the filter confusion.

Viability: **Very high.** This is a small packaging/layout change with low code risk.

### AUDIT-015: Hidden-column filter workflow is not discoverable

**Severity: High**  
**Confidence: High**  
**Release blocking: Yes for the advertised feature**

Evidence:

- The tester could not determine how to apply a hidden `Can be cultivated` filter.
- `ObjectTable_Filtering.cs:417-431` builds an add-filter menu from compatible columns.
- `ObjectTable_Filtering.cs:518-530` exposes that menu through a fixed-width `Add column filter` button beside Reset.
- `ObjectTable_Filtering.cs:533-543` only identifies a filter as hidden after it has already been added.
- There is no visible empty-state instruction explaining the sequence: open Filters, add a column filter, choose the hidden column, then configure its value.

Recommended implementation:

1. Replace the ambiguous empty state with an instructional panel and a primary `+ Add filter` button.
2. Rename the action from `Add column filter` to `Add filter` and add a tooltip: `Filter using visible or hidden columns`.
3. Make the add menu searchable when the compatible-column list is long.
4. Group options under `Visible columns` and `Hidden columns` or add a clear `(hidden)` suffix before selection.
5. Show the selected column name as a row heading, with the condition and value controls aligned underneath or to the right.
6. Add an obvious remove button on each filter row.
7. Keep `Reset all` visually secondary and separate from adding a filter.
8. When no filters exist, do not show only `No filters available`; distinguish `No filters applied` from `This table has no filterable columns`.
9. Add a short one-time hint or tooltip on the toolbar Filters control.

Viability: **High.** The underlying APIs already exist. Most work is layout, labels, grouping, and menu search rather than filter-engine changes.

### AUDIT-016: Expanded Values mode has poor density and unclear wording

**Severity: Medium**  
**Confidence: High**

Evidence:

- Screenshot 2 confirms the toggle works but produces large rows with unused space.
- `GUIStyles.cs:38-40` sets every expanded row to a fixed three-line height.
- `ObjectTable.cs:118` applies that height globally whenever expanded mode is on.
- `ObjectTable_Toolbar.cs:92-94` draws the Values toggle, and lines 122-125 switch the mode.

Recommended implementation:

1. Rename or tooltip the toggle as `Expand multi-value cells` and change the active tooltip to `Use compact values`.
2. Prefer per-row height based on the maximum visible line count in that row, capped at a sensible limit.
3. If variable row height is too invasive, reduce the fixed expanded height to two lines and allow overflow details in tooltips.
4. Review vertical alignment at 1.0×, 1.25×, and 1.5× UI scale.
5. Keep normal game font sizing unless a smaller style remains readable at all supported scales.

Viability: **Medium to high.** Wording and tooltip work is trivial. Variable row heights touch scrolling and row-position calculations and require careful regression testing.

### AUDIT-001: Repository and Workshop identity links are wrong

**Severity: High**  
**Confidence: High**

Evidence:

- `README.md:26` and `README.md:103` label original Workshop item `3479566439` as Stats Forked.
- `README.md:104-105` links `KyleMHB/Stats-Forked`, which is not the canonical repository.
- `About/About.xml:7` contains the same wrong repository URL.
- The current identifiers are Workshop `3712317437` and repository `KyleMHB/StatsForked`.

Fix viability: **Very high.** Correct the metadata and README links before release.

### AUDIT-002: Public version support claims are inconsistent

**Severity: High**  
**Confidence: High**

Local metadata correctly supports RimWorld 1.6 only through `About/About.xml:9-11` and `LoadFolders.xml:3-9`. The live Workshop description observed during the audit still claimed both 1.5 and 1.6, while its version tag and local package only support 1.6.

Fix viability: **Very high.** Publish the prepared 1.6-only `steam-description.md` after the release build is ready.

### AUDIT-003: Tested local build is not the public source state

**Severity: High**  
**Confidence: High**

Local `HEAD` is `3655cdf`, seven commits ahead of public `main` at `d3e2cd7`. The deployed build includes those commits, but the public repository and Workshop package do not yet establish the same state.

Fix viability: **High.** After release-blocking fixes and retesting, push the complete reviewed history, publish matching binaries, and tag or create a release so source and Workshop artifacts can be traced to the same commit.

### AUDIT-004: Owned and visible inventory semantics are incorrect

**Severity: High**  
**Confidence: High**

Evidence:

- `InventoryStateTracker.cs:51-67` scans all spawned things on each player-relevant map.
- `InventoryStateTracker.cs:79-86` adds every spawned thing definition to the owned set before any ownership or faction check.
- `InventoryStateTracker.cs:122-126` defines visible as spawned, mapped, and not forbidden. It does not check fog, line of sight, player ownership, storage, haulability, or accessibility.

Consequences:

- Enemy, wild, quest, or otherwise non-owned spawned objects can make `owned` true.
- An unfogged concept is implied by `visible`, but fog is not checked.
- Results can surprise users and make filter output unreliable.

Recommended implementation:

1. Write exact product definitions for `Owned` and `Visible on map` before changing code.
2. For owned items, count player-owned buildings, player pawns' equipment/apparel/inventory, and items held in player-owned storage or containers according to that definition.
3. For visible items, check the map fog grid or the relevant RimWorld visibility API, then separately decide whether forbidden items count.
4. Add scenario tests for enemy equipment, wild plants, fogged objects, forbidden stacks, caravan inventory, minified buildings, and container contents.

Viability: **Medium.** The fix is technically feasible but requires a clear semantic decision and more runtime scenarios than the other fixes.

### AUDIT-005: Multi-select filter options are persisted by translated label

**Severity: Medium**  
**Confidence: High**

`NTMFilter.cs:158-162` serializes `option.Label`, and `NTMFilter.cs:166-187` restores selection by matching labels. A language change, label change, or duplicate display label can break or misrestore a preset even though the surrounding filter now has a stable ID.

Recommended implementation: give each option a stable serialization key based on defName, enum value, canonical primitive value, or an explicit option ID. Retain label fallback for one migration cycle.

Viability: **High**, with moderate migration-care requirements.

### AUDIT-006: Table and event lifetimes can retain obsolete objects

**Severity: Medium**  
**Confidence: Medium-high**

- `TableWorker.cs:22` returns a new `ObjectTable` instance.
- `LabelColumnWorker.cs:57-63` subscribes a closure to the static `ResearchCompleted` event.
- `Events.cs:13-16` registers a receiver without a corresponding teardown path.

Repeatedly creating tables can retain workers or table state through static event subscriptions. No measured memory leak was produced, but the ownership pattern is unsafe.

Recommended implementation: use explicit subscribe/unsubscribe lifetime hooks, weak ownership, or one persistent table instance per worker. Add a repeated-open/close test and inspect subscriber count.

Viability: **Medium.** The fix is straightforward after table ownership is clarified.

### AUDIT-007: Automated and compatibility test coverage is absent

**Severity: Medium**  
**Confidence: High**

There is no automated test project or CI workflow. The current manual session materially improves confidence but does not cover persistence migrations, every table, CE, third-party integrations, multiple UI scales, or alternate languages.

Recommended implementation:

- Add CI builds for Debug and Release.
- Add XML integrity checks for duplicate defs, missing column references, and tagless compatibility columns.
- Add pure tests for preset serialization and stable option IDs.
- Keep a short manual matrix for game-only, every official DLC, CE, supported third-party mods, and UI scales 1.0/1.25/1.5.

Viability: **High** for build/XML checks, **medium** for in-game automation.

### AUDIT-008: Publisher Plus exclusions are stale

**Severity: Medium**  
**Confidence: High**

`_PublisherPlus.xml:12-19` excludes source and old PDB names for Core, Biotech, Anomaly, and CE, but omits Odyssey source and does not match the new `Stats.Compat.*.pdb` names. The custom deploy script safely removes PDBs, but Publisher Plus can ship unintended development files.

Fix viability: **Very high.** Update the exclusions or generate the publisher configuration from the same runtime manifest as deployment.

### AUDIT-009: Removed Odyssey fishing-outcomes feature remains documented and packaged as dead material

**Severity: Medium**  
**Confidence: High**

`CHANGELOG.md:45` says the Odyssey fishing outcomes table was removed, while `README.md:17`, `README.md:49`, and `README.md:67` still claim it exists. The current TableDef is absent, but its worker and column XML remain.

Recommended implementation: decide whether the feature is intentionally removed. If removed, delete dead worker/XML and correct all documentation. If intended to return, track it as a future feature and do not advertise it until its TableDef and runtime behavior are restored.

Viability: **Very high** for cleanup; **medium** for reimplementation.

### AUDIT-010: Inventory refresh can become expensive on large maps

**Severity: Medium**  
**Confidence: Medium**

When dirty, `InventoryStateTracker.cs:46-67` copies both sets and scans every spawned thing on relevant maps, then scans colonist equipment. This may be acceptable at the current throttle but has not been profiled on large colonies or mod-heavy maps.

Recommended implementation: first measure refresh frequency and duration. Only then consider incremental spawn/despawn updates, map lister APIs, or cached counts. Do not optimize blindly.

Viability: **Medium.** Measurement is easy; a correct incremental cache is more complex.

### AUDIT-011: Harmony patch identifier still uses the upstream identity

**Severity: Low**  
**Confidence: High**

`HarmonyPatches.cs:12` uses `Azzkiy.Stats`. A maintained fork should use a globally unique ID such as `KyleMHB.StatsForked` to avoid diagnostic and collision ambiguity.

Fix viability: **Very high.** Low-risk one-line change followed by startup testing.

### AUDIT-012: Deployment depends on a machine-local shared script

**Severity: Low**  
**Confidence: High**

`deploy.ps1:3-4` imports `..\_Shared\RimWorldModTools.ps1`, so a clean clone cannot deploy without an undocumented sibling repository component.

Recommended implementation: vendor the required helper, replace it with a repository-local script, or document and validate the dependency explicitly.

Viability: **High.** This is primarily release-engineering work.

### AUDIT-013: Incompatible-column diagnostic has stray dollar signs

**Severity: Low**  
**Confidence: High**

`ObjectTable.cs:152` interpolates `"${columnName}"`, `"${tableName}"`, and the worker name. C# interpolation already uses braces, so the emitted warning contains unwanted dollar signs.

Fix viability: **Very high.** Remove the three literal `$` characters and exercise the warning path once.

## Features and requests assessment

| Item | Classification | Current status | Worth doing? | Viability |
|---|---|---|---|---|
| Unique compatibility assembly names | Correctness/compatibility fix | Implemented and startup-tested | Yes | Complete, pending public release |
| Resizable columns | Usability feature | Implemented; test 2 passes | Yes | Complete |
| Session restoration | Usability feature | Implemented; test 5 passes | Yes | Complete |
| Saved/default presets | Usability feature | Implemented; automated coverage absent | Yes | High |
| Filter by hidden columns | Power-user feature | Implemented underneath, unusable in current flow | Yes, after UX redesign | High |
| Expanded multi-value cells | Readability feature | Works, but layout needs refinement | Yes | Medium-high |
| English localization | Required release quality | File exists but is not loaded | Mandatory | Very high |
| Inventory-aware owned/visible filters | Correctness feature | Semantics do not match names | Yes, after defining behavior | Medium |
| Stable preset option persistence | Reliability fix | Still label-based | Yes | High |
| Odyssey fishing outcomes | Previously removed feature | Documentation and dead assets conflict | Not until a concrete user need exists | Medium |
| Additional RimWorld 1.5 support | Compatibility expansion | Local package is 1.6-only | No for this release | Low value, high maintenance |

## Steam and GitHub user-signal summary

The Workshop audit synchronized 79 comments without collection failures. The recurring actionable themes were:

- compatibility and assembly-name collisions;
- saving table layouts and restoring sessions;
- default presets;
- filtering using columns that are not visible;
- column resizing;
- readability of cells with multiple values;
- confusion when Steam retains or fails to refresh an older local copy;
- requests and uncertainty around supported game versions and optional integrations.

The recent implementation addresses most of the underlying requested capability. The new runtime evidence shows that discoverability and packaging are now the limiting factors. A feature that users cannot locate should not be marked complete solely because its code path exists.

GitHub had no open user issues and no release artifacts or CI workflows at the time of review. That means Workshop comments remain the primary user-feedback source, while the repository currently provides weak release traceability.

## Performance assessment

No acute frame-time or startup performance problem was observed in the supplied test. The main areas worth measuring are:

1. Inventory cache refresh duration on large, object-heavy maps.
2. Filter evaluation cost with many active hidden and visible filters.
3. Table rebuild cost after research completion or inventory state changes.
4. Expanded-mode drawing and scrolling with many rows and variable multi-value cells.

Add lightweight development-only timing around these paths before redesigning them. Keep logs rate-limited and disabled in release builds.

## Compatibility and currency table

| Target | Declared/load state | Evidence | Review status |
|---|---|---|---|
| RimWorld 1.6 | Supported | About metadata, LoadFolders, Player.log | Verified in game |
| RimWorld 1.5 | Not locally supported | No 1.5 load folder or metadata | Remove public claim |
| Royalty | Base-game DLC | Active in Player.log | Startup verified |
| Ideology | Base-game DLC | Active in Player.log | Startup verified |
| Biotech | Conditional module | Active in Player.log | Startup verified |
| Anomaly | Conditional module | Active in Player.log | Startup verified |
| Odyssey | Conditional module | Active in Player.log | Startup verified; dead fishing material remains |
| Combat Extended | Conditional module | Not active in supplied log | Build-only evidence; runtime unverified here |
| Quality Framework | Third-party mod | Active in Player.log | Co-load startup verified |
| Fish Traps integration | Third-party compatibility concern | No explicit result supplied | Unverified |
| Searchable Defs integration | Third-party compatibility concern | No explicit result supplied | Unverified |

## Validation already completed

- Debug build: succeeded with 0 errors and 9 existing warnings.
- Release build: succeeded with 0 errors and 9 existing warnings.
- Deployment: succeeded to the local RimWorld Mods directory.
- Release DLL hashes matched the deployed DLLs.
- Deployed compatibility assemblies use unique names:
  - `Core.dll`
  - `Stats.Compat.CE.dll`
  - `Stats.Compat.Biotech.dll`
  - `Stats.Compat.Anomaly.dll`
  - `Stats.Compat.Odyssey.dll`
- Old generic compatibility DLL names were absent from the deployment.
- 220 XML files parsed successfully.
- 209 ColumnDefs and 21 TableDefs were checked.
- No duplicate defNames, tagless columns, or missing table-column references were found.
- All 59 defined localization constants had corresponding English keyed entries.
- Player.log startup check passed for Core plus all official DLCs and Quality Framework.
- Manual test 2 passed.
- Manual test 5 passed.
- Manual test 6 functionally switched modes and displayed additional values.

## Validation gaps

- Hidden-filter creation and preset restoration are not runtime-validated.
- The localization layout has not been corrected or retested.
- Combat Extended was not active in the supplied Player.log.
- Third-party Fish Traps and Searchable Defs results were not separately evidenced.
- No language other than English was assessed.
- No automated tests or CI runs exist.
- No large-colony performance profile exists.
- No repeated table open/close lifetime test exists.
- Public Workshop files were not compared byte-for-byte with local revision `3655cdf` because that revision has not been published.

## Prioritized roadmap

### Release-blocking batch

1. Fix localization placement under the always-loaded Core content root.
2. Verify every toolbar, filter, preset, and context-menu label renders as English text at 1.0× and 1.5× UI scale.
3. Redesign the empty and add-filter flow enough that a first-time user can add `Can be cultivated` without instructions.
4. Re-run tests 3 and 4 end to end.
5. Refine the Values label/tooltip and reduce expanded-mode wasted height, then re-run test 6.
6. Correct canonical repository and Workshop URLs.
7. Make all public support text say RimWorld 1.6 only.
8. Update `_PublisherPlus.xml` and confirm a staged package contains no source, PDBs, or obsolete generic DLLs.

### First post-release correctness batch

1. Define and correct owned/visible inventory semantics.
2. Replace label-based filter option persistence with stable IDs plus migration fallback.
3. Give Harmony a unique fork-specific identifier.
4. Fix the incompatible-column diagnostic string.
5. Remove or restore the Odyssey fishing-outcomes dead material and align documentation.

### Engineering hardening batch

1. Add CI builds and XML integrity validation.
2. Add serialization tests for presets, hidden filters, defaults, and session state.
3. Clarify table/event lifetime and unsubscribe static handlers.
4. Profile inventory refresh and large-table rendering.
5. Make deployment self-contained and produce a traceable release artifact tied to a commit.

## Safest first implementation batch

The safest next code batch for another agent is deliberately narrow:

1. Relocate `StatsForked.xml` into `Core/Languages/English/Keyed` and update deployment validation.
2. Correct the repository URLs and Workshop IDs in `About.xml` and README.
3. Correct `_PublisherPlus.xml` for Odyssey and current assembly/PDB names.
4. Change filter empty-state copy and make `+ Add filter` the primary action.
5. Add visible/hidden grouping and a `(hidden)` indicator in the add-filter menu.
6. Rename/tool-tip the Values action without changing row-height calculations yet.
7. Change the Harmony ID and malformed warning string.
8. Build Debug and Release, deploy, clear or replace the old local mod copy, and perform tests 1, 3, 4, and 6.

Do not combine the inventory semantic rewrite or variable-height row system into this batch. Those changes have broader behavioral and layout risk and deserve separate testing.

## Direct answers to the tester

- **Does test 1 pass?** Yes, for the active official-DLC and Quality Framework configuration. The log is clean of Stats Forked errors and duplicate assemblies.
- **Is `Values` the button for test 6?** Yes. Red X is compact; green check is expanded.
- **Did test 6 pass?** Functionally yes, visually only partially. Expanded content appears, but the fixed row height and labeling need improvement.
- **Is the top-row font actually wrong?** The main confirmed problem is that raw translation keys are being rendered. Fix translation loading first, then reassess typography at 1.5× scale.
- **Why is hidden filtering hard to use?** The action is buried inside the Filters window, the empty state does not teach the workflow, and hidden columns are not clearly grouped before selection. That is a genuine UX defect.
- **What should be tested after the next fix?** Verify clean English toolbar labels, add a hidden `Can be cultivated = Yes` filter, save and reload it in a preset, restart and verify the session state, and compare compact/expanded values at 1.0× and 1.5× UI scale.

## Final assessment

The recent implementation is directionally worthwhile and most requested capabilities are viable. The build and official-DLC startup foundation are healthy. The immediate problem is not a deep architectural failure: it is a combination of one high-impact packaging mistake and an under-designed filter interaction.

Fix those two areas before publishing. After that, correct release identity and packaging metadata, then handle the inventory semantics and stable preset serialization as the next correctness-focused release.
