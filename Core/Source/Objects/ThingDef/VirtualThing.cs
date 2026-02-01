namespace Stats.Objects.ThingDef;

public readonly record struct VirtualThing(Verse.ThingDef Def, Verse.ThingDef? StuffDef = null);
