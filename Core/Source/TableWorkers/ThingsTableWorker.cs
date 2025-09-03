using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

public abstract class ThingsTableWorker<TThing> : TableWorker<TThing>, TableWorker<TThing>.IStreaming where TThing : Thing
{
    public event Action<TThing>? OnObjectAdded;
    public event Action<TThing>? OnObjectRemoved;
    public sealed override IEnumerable<TThing> InitialObjects
    {
        get
        {
            foreach (var map in Find.Maps)
            {
                foreach (var thing in map.spawnedThings)
                {
                    if (thing is TThing tthing && IsValidThing(tthing))
                    {
                        yield return tthing;
                    }
                }
            }
        }
    }
    public ThingsTableWorker(TableDef tableDef) : base(tableDef)
    {
        Globals.Events.ThingSpawned += thing =>
        {
            if (thing is TThing tthing && IsValidThing(tthing))
            {
                OnObjectAdded?.Invoke(tthing);
            }
        };
        Globals.Events.ThingDespawned += thing =>
        {
            if (thing is TThing tthing && IsValidThing(tthing))
            {
                OnObjectRemoved?.Invoke(tthing);
            }
        };
    }
    protected abstract bool IsValidThing(TThing thing);
}
