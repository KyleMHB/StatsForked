using System;
using System.Collections.Generic;
using Verse;

namespace Stats;

public sealed class HumanlikeTableWorker : TableWorker<Humanlike>, TableWorker<Humanlike>.IStreaming
{
    public event Action<Humanlike>? OnObjectAdded;
    public event Action<Humanlike>? OnObjectRemoved;
    public override IEnumerable<Humanlike> InitialObjects
    {
        get
        {
            foreach (var thing in Find.Maps.GetSpawnedThings())
            {
                if (thing is Pawn pawn && pawn.def.race?.Humanlike == true)
                {
                    yield return new Humanlike(pawn, pawn.def.race);
                }
            }
        }
    }
    public HumanlikeTableWorker(TableDef tableDef) : base(tableDef)
    {
        Globals.Events.ThingSpawned += thing =>
        {
            if (thing is Pawn pawn && pawn.def.race?.Humanlike == true)
            {
                OnObjectAdded?.Invoke(new Humanlike(pawn, pawn.def.race));
            }
        };
        Globals.Events.ThingDespawned += thing =>
        {
            if (thing is Pawn pawn && pawn.def.race?.Humanlike == true)
            {
                OnObjectRemoved?.Invoke(new Humanlike(pawn, pawn.def.race));
            }
        };
    }
}
