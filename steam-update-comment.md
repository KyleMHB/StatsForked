Stats Forked has been updated for the latest maintenance and compatibility fixes.

Changes:
- Compatibility assemblies now use unique filenames, avoiding collisions with other mods that use generic assembly names. This specifically prevents the Fish Traps/Odyssey assembly collision.
- Combat Extended compatibility columns now have the required tags, fixing no-tag errors and restoring their visibility in the column menu.
- Table windows now restore their open tables, order, active tab, and saved default presets.
- The Filters window now has a guided Add filter workflow with search and separate table, visible-column, and hidden-column sections.
- Active filters remain available while their source column is hidden, and each filter now has a clear remove action.
- Window resizing is stable, and expanded multi-value cells use denser two-line rows with complete details in tooltips.
- Toolbar, filter, preset, and help labels now load correctly in English instead of displaying raw translation keys.

Important: fully restart RimWorld after updating. The optional compatibility assembly filenames changed, and a complete restart is required for the new files to load correctly.

If problems persist, please report them in the [url=https://github.com/KyleMHB/StatsFork/issues]GitHub issue tracker[/url] and include a HugsLib log or Player.log together with your full mod order.
