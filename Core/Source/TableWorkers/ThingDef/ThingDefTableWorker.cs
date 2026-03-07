using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.TableWorkers.ThingDef;

public abstract class ThingDefTableWorker :
    TableWorker<DefBasedObject>,
    IRefRecordsProvider<Verse.ThingDef>,
    IRefRecordsProvider<Verse.Def>
{
    public override List<DefBasedObject> InitialObjects { get; }
    IEnumerable<Verse.ThingDef> IRefRecordsProvider<Verse.ThingDef>.Records => InitialObjects.Select(@object => (Verse.ThingDef)@object.Def);
    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialObjects.Select(@object => @object.Def);

    public override event Action<DefBasedObject>? OnObjectAdded;
    public override event Action<DefBasedObject>? OnObjectRemoved;

    protected ThingDefTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<DefBasedObject> initialObjects = new(250);
        foreach (Verse.ThingDef thingDef in DefDatabase<Verse.ThingDef>.AllDefsListForReading)
        {
            if (IsValidThingDef(thingDef))
            {
                HashSet<Verse.ThingDef>? stuffDefs = thingDef.GetAllowedStuffs();

                if (stuffDefs?.Count > 0)
                {
                    foreach (Verse.ThingDef stuffDef in stuffDefs)
                    {
                        DefBasedObject @object = new(thingDef, stuffDef);
                        initialObjects.Add(@object);
                    }
                }
                else
                {
                    DefBasedObject @object = new(thingDef);
                    initialObjects.Add(@object);
                }
            }
        }
        InitialObjects = initialObjects;
    }

    protected abstract bool IsValidThingDef(Verse.ThingDef thingDef);
}
