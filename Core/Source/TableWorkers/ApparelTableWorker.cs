using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class ApparelTableWorker : TableWorker<Apparel>, TableWorker<Apparel>.IStreaming
{
    public event Action<Apparel>? OnObjectAdded;
    public event Action<Apparel>? OnObjectRemoved;
    public sealed override IEnumerable<Apparel> InitialObjects
    {
        get
        {
            foreach (var thing in Find.Maps.GetSpawnedThings())
            {
                if (thing.def.apparel != null)
                {
                    yield return new Apparel(thing, thing.def.apparel);
                }
            }
        }
    }
    public ApparelTableWorker(TableDef tableDef) : base(tableDef)
    {
        Globals.Events.ThingSpawned += thing =>
        {
            if (thing.def.apparel != null)
            {
                OnObjectAdded?.Invoke(new Apparel(thing, thing.def.apparel));
            }
        };
        Globals.Events.ThingDespawned += thing =>
        {
            if (thing.def.apparel != null)
            {
                OnObjectRemoved?.Invoke(new Apparel(thing, thing.def.apparel));
            }
        };
    }
}
