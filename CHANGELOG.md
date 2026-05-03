# Changelog

## 2026-05-03

### Fixed

- Fixed missing table icon paths logging RimWorld load errors by falling back to the default bad texture.

### Added

- Added completed Odyssey gameplay table support for books, fish, gravship systems, orbital infrastructure, unique weapons, and fishing outcomes.

### Changed

- Updated README and Steam Workshop description support text to state that Stats Forked supports RimWorld 1.6 only.
- Clarified the project history now that RimWorld 1.5 support is no longer maintained.

## 2026-05-02

### Added

- Added updated README and Steam Workshop description files for the documentation refresh.
- Added a template-structured README covering installation, usage, configuration, source builds, validation, links, license, and credits.
- Added a Steam Workshop description document for publishing-facing mod copy.
- Added a dated changelog structure based on the GlobalTemplates changelog format.

### Changed

- Changed the changelog from a running summary into a dated project history with categorized entries.
- Updated documentation to use the current package metadata, Steam Workshop link, supported RimWorld versions, compatibility modules, and source repository links.

## 2026-05-01

### Added

- Added inventory-aware table support for colony-owned and map-visible item filters.
- Added saved table presets for restoring filter state, visible columns, and variant display mode.
- Added stuffable item variant support with a table toolbar toggle.
- Added a bionics comparison table with body part, capacity, efficiency, effect, special effect, and content source columns.
- Added distance-based ranged DPS columns for close, short, medium, and long range comparisons.
- Added general stats table definitions and expanded table definitions for Core, Biotech, Anomaly, CE, and Odyssey content.
- Added Biotech gene, mechanoid, and apparel table support.
- Added Combat Extended columns for caliber, one-handed weapons, magazine capacity, reload time, and related weapon stats.
- Added an Odyssey compatibility module scaffold and included it in the solution and load folder configuration.

### Changed

- Changed the project branding and metadata to Stats Forked.
- Updated the table UI for column management, filtering, sorting, row handling, toolbar actions, variant expansion, and preset persistence.
- Updated stat request handling to use default stuff for apparel where needed.
- Updated equipped stat offset handling and quality-aware stat table behavior.
- Split Biotech compatibility out of Core into a dedicated module.
- Updated runtime packaging so builds produce `Runtime Only/Stats Forked`.
- Updated repository ignore rules for generated assemblies, object folders, build outputs, local caches, debug symbols, Visual Studio files, and temporary files.

### Fixed

- Fixed compatibility table structure for newer RimWorld content and active module loading.
- Fixed generated build and deploy output being kept in the working tree.

### Internal

- Added shared MSBuild configuration in `Directory.Build.props`.
- Added Odyssey, CE, Biotech, Anomaly, and Core projects to the solution/build layout.
- Preserved the tracked Combat Extended reference DLL needed by the project.
