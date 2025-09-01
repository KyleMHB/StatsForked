using Verse;

namespace Stats;

/*

Notes:

---

Basically a lighter version of RimWorld.StatRequest.

---

Using structs results in less (long-lived) objects allocated on heap. Which means less work for GC.

---

For (global) caches to work we need value equality:

var thingDef = DefDatabase<ThingDef>.GetNamed("...");
var stuffDef = DefDatabase<ThingDef>.GetNamed("...");

var thing1 = new ThingAlike(thingDef, stuffDef);
var thing2 = new ThingAlike(thingDef, stuffDef);

thing1 == thing2;// true

*/

public readonly record struct AbstractThing
{
    public ThingDef Def { get; }
    public ThingDef? StuffDef { get; }
    public AbstractThing(ThingDef def, ThingDef? stuffDef = null)
    {
        Def = def;
        StuffDef = stuffDef;
    }
}
