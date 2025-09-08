using RimWorld;
using Verse;

namespace Stats;

public readonly record struct ApparelDef(ThingDef Def, ApparelProperties Props, ThingDef? StuffDef = null);
public readonly record struct Apparel(Thing Thing, ApparelProperties Props);
public readonly record struct Humanlike(Pawn Pawn, RaceProperties RaceProps);
public readonly record struct RangedWeaponDef(ThingDef Def, ThingDef? StuffDef = null);
public readonly record struct TurretDef(ThingDef Def, BuildingProperties BuildingProps, ThingDef GunDef, ThingDef? StuffDef = null);
