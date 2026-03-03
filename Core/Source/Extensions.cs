using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using Stats.ObjectTable;
using UnityEngine;
using Verse;

namespace Stats;

public static class UnityEngine_Rect_Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect CutByX(ref this Rect rect, float amount)
    {
        Rect result = rect with { width = amount };
        // Changing "xMin" also auto corrects width. Changing "x" doesn't.
        rect.xMin += amount;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    internal static Rect CutByY(ref this Rect rect, float amount)
    {
        Rect result = rect with { height = amount };
        // Changing "yMin" also auto corrects height. Changing "y" doesn't.
        rect.yMin += amount;

        return result;
    }

    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Rect ContractedByObjectTableCellPadding(this Rect rect)
    {
        return rect.ContractedBy(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
    }
}

public static class Verse_VerbProperties_List_Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(static verb => verb?.isPrimary == true);
    }
}

public static class System_String_Extensions
{
    internal static Color ToUniqueColorRGB(this string str)
    {
        // I don't think that this hash is very reliable.
        uint hash = (uint)str.GetHashCode();
        float r = ((hash & 0xFF000000) >> 24) / 255f;
        float g = ((hash & 0x00FF0000) >> 16) / 255f;
        float b = ((hash & 0x0000FF00) >> 8) / 255f;

        return new Color(r, g, b);
    }

    internal static bool Contains(this string str, string substr, StringComparison comp)
    {
        return str.IndexOf(substr, comp) >= 0;
    }
}

public static class UnityEngine_Color_Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= Globals.GUI.Opacity;

        return color;
    }
}

public static class System_Function_Extensions
{
    // Memoize every function call.
    public static Func<TArg, TRes> Memoized<TArg, TRes>(this Func<TArg, TRes> function)
    {
        Dictionary<TArg, TRes> cache = [];

        return (TArg arg) =>
        {
            bool resultIsCached = cache.TryGetValue(arg, out TRes? result);

            if (resultIsCached)
            {
                return result;
            }

            return cache[arg] = function(arg);
        };
    }
}

public static class System_Single_Extensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static decimal ToDecimal(this float value, int digits)
    {
        // "When you convert float or double to decimal, the source value is converted
        // to decimal representation and rounded to the nearest number after
        // the 28th decimal place if necessary."
        //
        // Keyword is "if".
        return (decimal)Math.Round(value, digits);
    }
}

public static class Verse_ThingDef_Extensions
{
    private static readonly Dictionary<ThingDef, HashSet<RecipeDef>> _thingDefRecipes = [];
    private static readonly Dictionary<ThingDef, HashSet<PawnKindDef>> _pawnKinds = [];
    private static readonly Dictionary<string, HashSet<ThingDef>> _weaponsByTag = [];
    private static readonly Dictionary<ThingDef, HashSet<ResearchProjectDef>> _researchProjects = [];

    static Verse_ThingDef_Extensions()
    {
        foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            ThingDef producedThingDef = recipeDef.ProducedThingDef;

            if (producedThingDef != null)
            {
                bool recipesEntryExists = _thingDefRecipes.TryGetValue(producedThingDef, out HashSet<RecipeDef>? recipeDefs);

                if (recipesEntryExists)
                {
                    recipeDefs.Add(recipeDef);
                }
                else
                {
                    _thingDefRecipes[producedThingDef] = [recipeDef];
                }
            }
        }

        foreach (PawnKindDef pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
        {
            if (pawnKindDef.race == null)
            {
                continue;
            }

            _pawnKinds.TryGetValue(pawnKindDef.race, out HashSet<PawnKindDef>? pawnKindDefs);

            if (pawnKindDefs != null)
            {
                pawnKindDefs.Add(pawnKindDef);
            }
            else
            {
                _pawnKinds[pawnKindDef.race] = [pawnKindDef];
            }
        }

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.IsWeapon && thingDef.weaponTags?.Count > 0)
            {
                foreach (string tag in thingDef.weaponTags)
                {
                    _weaponsByTag.TryGetValue(tag, out HashSet<ThingDef>? weapons);

                    if (weapons != null)
                    {
                        weapons.Add(thingDef);
                    }
                    else
                    {
                        _weaponsByTag[tag] = [thingDef];
                    }
                }
            }
        }

        foreach (ResearchProjectDef researchProjectDef in DefDatabase<ResearchProjectDef>.AllDefsListForReading)
        {
            foreach (Def def in researchProjectDef.UnlockedDefs)
            {
                if (def is ThingDef thingDef)
                {
                    _researchProjects.TryGetValue(thingDef, out HashSet<ResearchProjectDef>? researchProjects);

                    if (researchProjects != null)
                    {
                        researchProjects.Add(researchProjectDef);
                    }
                    else
                    {
                        _researchProjects[thingDef] = [researchProjectDef];
                    }
                }
            }
        }
    }

    // GenStuff.DefaultStuffFor() is a bit too heavy for some tasks.
    private static readonly Func<ThingDef, ThingDef?> GetDefaultStuffCached =
    System_Function_Extensions.Memoized((ThingDef thingDef) =>
    {
        return GenStuff.DefaultStuffFor(thingDef);
    });

    // Verse.ThingDef.defaultStuff isn't actually what is advertised.
    public static ThingDef? GetDefaultStuff(this ThingDef thingDef)
    {
        return GetDefaultStuffCached(thingDef);
    }

    public static bool IsBuildingObtainableByPlayer(this ThingDef thingDef)
    {
        return thingDef.BuildableByPlayer || thingDef.Minifiable;
    }

    public static HashSet<RecipeDef>? GetRecipeDefs(this ThingDef thingDef)
    {
        _thingDefRecipes.TryGetValue(thingDef, out HashSet<RecipeDef>? recipeDefs);

        return recipeDefs;
    }

    public static float GetStatValuePerceived(this ThingDef thingDef, StatDef statDef, ThingDef? stuffDef = null)
    {
        StatRequest statRequest = StatRequest.For(thingDef, stuffDef);

        if (statDef.Worker.ShouldShowFor(statRequest))
        {
            return statDef.Worker.GetValue(statRequest);
        }

        return 0f;
    }

    public static HashSet<PawnKindDef>? GetPawnKindDefs(this ThingDef thingDef)
    {
        _pawnKinds.TryGetValue(thingDef, out HashSet<PawnKindDef>? pawnKindDefs);

        return pawnKindDefs;
    }

    // TODO: Cache this too?
    public static HashSet<ThingDef>? GetPossibleWeapons(this ThingDef thingDef)
    {
        HashSet<PawnKindDef>? pawnKindDefs = thingDef.GetPawnKindDefs();

        if (pawnKindDefs?.Count > 0)
        {
            HashSet<ThingDef> result = [];

            foreach (PawnKindDef pawnKindDef in pawnKindDefs)
            {
                if (pawnKindDef.weaponTags != null)
                {
                    foreach (string tag in pawnKindDef.weaponTags)
                    {
                        _weaponsByTag.TryGetValue(tag, out HashSet<ThingDef>? weapons);

                        if (weapons != null)
                        {
                            result.AddRange(weapons);
                        }
                    }
                }
            }

            return result;
        }

        return null;
    }

    public static HashSet<ResearchProjectDef>? GetResearchProjectDefs(this ThingDef thingDef)
    {
        _researchProjects.TryGetValue(thingDef, out HashSet<ResearchProjectDef>? researchProjectDefs);

        return researchProjectDefs;
    }

    public static HashSet<ThingDef>? GetAllowedStuffs(this ThingDef thingDef)
    {
        if (thingDef.stuffCategories?.Count > 0)
        {
            HashSet<ThingDef> result = new(30);

            foreach (StuffCategoryDef stuffCategoryDef in thingDef.stuffCategories)
            {
                result.AddRange(stuffCategoryDef.GetStuffDefs());
            }

            return result;
        }

        return null;
    }

    public static ThingDef TurretGunDefOrSelf(this ThingDef thingDef)
    {
        return thingDef.building?.turretGunDef ?? thingDef;
    }
}

public static class Verse_RecipeDef_Extensions
{
    private static readonly Dictionary<RecipeDef, HashSet<ThingDef>> _recipeUsers = [];

    static Verse_RecipeDef_Extensions()
    {
        foreach (RecipeDef recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            if (recipeDef.recipeUsers?.Count > 0)
            {
                bool recipeUsersEntryExists = _recipeUsers.TryGetValue(recipeDef, out HashSet<ThingDef>? recipeUsers);

                if (recipeUsersEntryExists)
                {
                    recipeUsers.AddRange(recipeDef.recipeUsers);
                }
                else
                {
                    _recipeUsers[recipeDef] = [.. recipeDef.recipeUsers];
                }
            }
        }

        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.recipes?.Count > 0)
            {
                foreach (RecipeDef? recipeDef in thingDef.recipes)
                {
                    bool recipeUsersEntryExists = _recipeUsers.TryGetValue(recipeDef, out HashSet<ThingDef>? recipeUsers);

                    if (recipeUsersEntryExists)
                    {
                        recipeUsers.Add(thingDef);
                    }
                    else
                    {
                        _recipeUsers[recipeDef] = [thingDef];
                    }
                }
            }
        }
    }

    public static HashSet<ThingDef>? GetAllRecipeUsers(this RecipeDef recipeDef)
    {
        _recipeUsers.TryGetValue(recipeDef, out HashSet<ThingDef>? recipeUsers);

        return recipeUsers;
    }
}

public static class RimWorld_CompProperties_EggLayer_Extensions
{
    public static ThingDef GetAnyEggDef(this CompProperties_EggLayer compProps)
    {
        return compProps.eggUnfertilizedDef ?? compProps.eggFertilizedDef;
    }
}

public static class RimWorld_PlantProperties_Extensions
{
    public static float GetGrowDaysActual(this PlantProperties plantProperties)
    {
        // Source: Wiki
        return plantProperties.growDays / 0.5417f;
    }
}

public static class Verse_Map_List_Extensions
{
    public static IEnumerable<Thing> GetSpawnedThings(this List<Map> maps)
    {
        foreach (Map map in maps)
        {
            foreach (Thing thing in map.spawnedThings)
            {
                yield return thing;
            }
        }
    }
}

public static class RimWorld_StuffCategoryDef_Extensions
{
    private static readonly Dictionary<StuffCategoryDef, HashSet<ThingDef>> _stuffsByCategory = [];

    static RimWorld_StuffCategoryDef_Extensions()
    {
        foreach (ThingDef thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.stuffProps == null) continue;

            foreach (StuffCategoryDef stuffCategoryDef in thingDef.stuffProps.categories)
            {
                bool exists = _stuffsByCategory.TryGetValue(stuffCategoryDef, out HashSet<ThingDef>? categoryStuffs);

                if (exists)
                {
                    categoryStuffs.Add(thingDef);
                }
                else
                {
                    _stuffsByCategory[stuffCategoryDef] = [thingDef];
                }
            }
        }
    }

    public static HashSet<ThingDef> GetStuffDefs(this StuffCategoryDef stuffCategoryDef)
    {
        return _stuffsByCategory[stuffCategoryDef];
    }
}

public static class System_Collections_Generic_List_Extensions
{
    public static void ReplaceWithLast<T>(this List<T> list, int index)
    {
        int lastItemlIndex = list.Count - 1;
        list[index] = list[lastItemlIndex];
        list.RemoveAt(lastItemlIndex);
    }
}
