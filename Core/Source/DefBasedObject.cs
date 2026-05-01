using RimWorld;

namespace Stats;

public readonly record struct DefBasedObject
{
    public readonly Verse.Thing? Thing;
    public readonly Verse.Def Def;
    public readonly Verse.ThingDef? StuffDef;
    public readonly QualityCategory Quality;

    public DefBasedObject(Verse.Thing thing)
    {
        Thing = thing;
        Def = Thing.def;
        StuffDef = thing.Stuff;
        Quality = QualityCategory.Normal;
    }

    public DefBasedObject(Verse.Def def, Verse.ThingDef? stuffDef = null, QualityCategory quality = QualityCategory.Normal)
    {
        Def = def;
        StuffDef = stuffDef;
        Quality = quality;
    }

    public DefBasedObject WithQuality(QualityCategory quality)
    {
        return new DefBasedObject(Def, StuffDef, quality);
    }
}
