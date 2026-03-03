namespace Stats.Objects.DefBased;

public readonly record struct DefBasedObject
{
    public readonly Verse.Thing? Thing;
    public readonly Verse.Def Def;
    public readonly Verse.ThingDef? StuffDef;

    public DefBasedObject(Verse.Thing thing)
    {
        Thing = thing;
        Def = Thing.def;
        StuffDef = thing.Stuff;
    }

    public DefBasedObject(Verse.Def def, Verse.ThingDef? stuffDef = null)
    {
        Def = def;
        StuffDef = stuffDef;
    }
}
