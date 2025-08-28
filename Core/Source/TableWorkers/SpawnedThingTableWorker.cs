using System.Collections.Generic;
using Stats.Widgets;
using Verse;

namespace Stats;

public abstract class SpawnedThingTableWorker : TableWorker<ThingAlike>
{
    protected sealed override IEnumerable<ThingAlike> Records
    {
        get
        {
            foreach (var map in Find.Maps)
            {
                foreach (var thing in map.spawnedThings)
                {
                    if (IsValidThing(thing))
                    {
                        yield return new(thing);
                    }
                }
            }
        }
    }
    public SpawnedThingTableWorker(TableDef tableDef) : base(tableDef)
    {
    }
    protected override ObjectTable<ThingAlike> MakeTableWidget()
    {
        var widget = base.MakeTableWidget();

        Globals.Events.ThingSpawned += thing =>
        {
            if (IsValidThing(thing))
            {
                widget.AddObject(new(thing));
            }
        };
        Globals.Events.ThingDespawned += thing => widget.RemoveObject(new(thing));

        return widget;
    }
    protected abstract bool IsValidThing(Thing thing);
}
