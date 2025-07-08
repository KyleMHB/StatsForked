using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using RimWorld;
using UnityEngine;
using Verse;

namespace Stats;

public static class UnityEngineRectExtensions
{
    internal static Rect CutByX(ref this Rect rect, float amount)
    {
        var result = rect with { width = amount };
        // Changing "xMin" also auto corrects width. Changing "x" doesn't.
        rect.xMin += amount;

        return result;
    }
    internal static Rect CutByY(ref this Rect rect, float amount)
    {
        var result = rect with { height = amount };
        // Changing "yMin" also auto corrects height. Changing "y" doesn't.
        rect.yMin += amount;

        return result;
    }
}

public static class VerseVerbPropertiesListExtensions
{
    public static VerbProperties? Primary(this List<VerbProperties> verbs)
    {
        return verbs.FirstOrFallback(static verb => verb?.isPrimary == true);
    }
}

public static class StringExtensions
{
    internal static Color ToUniqueColorRGB(this string str)
    {
        // I don't think that this hash is very reliable.
        var hash = (uint)str.GetHashCode();
        var r = ((hash & 0xFF000000) >> 24) / 255f;
        var g = ((hash & 0x00FF0000) >> 16) / 255f;
        var b = ((hash & 0x0000FF00) >> 8) / 255f;

        return new Color(r, g, b);
    }
    internal static bool Contains(this string str, string substr, StringComparison comp)
    {
        return str.IndexOf(substr, comp) >= 0;
    }
}

public static class UnityEngineColorExtensions
{
    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    public static Color AdjustedForGUIOpacity(this Color color)
    {
        color.a *= Globals.GUI.Opacity;

        return color;
    }
}

public static class FunctionExtensions
{
    // Memoize every function call.
    public static Func<TArg, TRes> Memoized<TArg, TRes>(this Func<TArg, TRes> function)
    {
        var cache = new Dictionary<TArg, TRes>();

        return (TArg arg) =>
        {
            var resultIsCached = cache.TryGetValue(arg, out var result);

            if (resultIsCached)
            {
                return result;
            }

            return cache[arg] = function(arg);
        };
    }
}

public static class SystemSingleExtensions
{
    public static decimal ToDecimal(this float value, int digits)
    {
        // "When you convert float or double to decimal, the source value is converted
        // to decimal representation and rounded to the nearest number after
        // the 28th decimal place if necessary."
        //
        // Key word is "if".
        return (decimal)Math.Round(value, digits);
    }
}

public static class VerseThingDefExtensions
{
    private static readonly Dictionary<ThingDef, HashSet<RecipeDef>> ThingDefRecipes = [];
    private static readonly Dictionary<ThingDef, HashSet<PawnKindDef>> PawnKinds = [];
    private static readonly Dictionary<string, HashSet<ThingDef>> WeaponsByTag = [];
    static VerseThingDefExtensions()
    {
        foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            var producedThingDef = recipeDef.ProducedThingDef;

            if (producedThingDef != null)
            {
                var recipesEntryExists = ThingDefRecipes.TryGetValue(producedThingDef, out var recipeDefs);

                if (recipesEntryExists)
                {
                    recipeDefs.Add(recipeDef);
                }
                else
                {
                    ThingDefRecipes[producedThingDef] = [recipeDef];
                }
            }
        }

        foreach (var pawnKindDef in DefDatabase<PawnKindDef>.AllDefsListForReading)
        {
            if (pawnKindDef.race == null)
            {
                continue;
            }

            PawnKinds.TryGetValue(pawnKindDef.race, out var pawnKindDefs);

            if (pawnKindDefs != null)
            {
                pawnKindDefs.Add(pawnKindDef);
            }
            else
            {
                PawnKinds[pawnKindDef.race] = [pawnKindDef];
            }
        }

        foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.IsWeapon && thingDef.weaponTags?.Count > 0)
            {
                foreach (var tag in thingDef.weaponTags)
                {
                    WeaponsByTag.TryGetValue(tag, out var weapons);

                    if (weapons != null)
                    {
                        weapons.Add(thingDef);
                    }
                    else
                    {
                        WeaponsByTag[tag] = [thingDef];
                    }
                }
            }
        }
    }
    // GenStuff.DefaultStuffFor() is a bit too heavy for some tasks.
    private static readonly Func<ThingDef, ThingDef?> GetDefaultStuffCached =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return GenStuff.DefaultStuffFor(thingDef);
    });
    // Verse.ThingDef.defaultStuff isn't actually what is advertised.
    public static ThingDef? GetDefaultStuff(this ThingDef thingDef)
    {
        return GetDefaultStuffCached(thingDef);
    }
    private static readonly Func<ThingDef, CompProperties_Power?> GetPowerCompPropertiesCached =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.GetCompProperties<CompProperties_Power>();
    });
    public static CompProperties_Power? GetPowerCompProperties(this ThingDef thingDef)
    {
        return GetPowerCompPropertiesCached(thingDef);
    }
    private static readonly Func<ThingDef, CompProperties_Refuelable?> GetRefuelableCompPropertiesCached =
    FunctionExtensions.Memoized((ThingDef thingDef) =>
    {
        return thingDef.GetCompProperties<CompProperties_Refuelable>();
    });
    public static CompProperties_Refuelable? GetRefuelableCompProperties(this ThingDef thingDef)
    {
        return GetRefuelableCompPropertiesCached(thingDef);
    }
    public static bool IsBuildingObtainableByPlayer(this ThingDef thingDef)
    {
        return thingDef.BuildableByPlayer || thingDef.Minifiable;
    }
    public static HashSet<RecipeDef>? GetRecipeDefs(this ThingDef thingDef)
    {
        ThingDefRecipes.TryGetValue(thingDef, out var recipeDefs);

        return recipeDefs;
    }
    public static float GetStatValuePerceived(this ThingDef thingDef, StatDef statDef, ThingDef? stuffDef = null)
    {
        var statRequest = StatRequest.For(thingDef, stuffDef);

        if (statDef.Worker.ShouldShowFor(statRequest))
        {
            return statDef.Worker.GetValue(statRequest);
        }

        return 0f;
    }
    public static HashSet<PawnKindDef>? GetPawnKindDefs(this ThingDef thingDef)
    {
        PawnKinds.TryGetValue(thingDef, out var pawnKindDefs);

        return pawnKindDefs;
    }
    // TODO: Cache this too?
    public static HashSet<ThingDef>? GetPossibleWeapons(this ThingDef thingDef)
    {
        var pawnKindDefs = thingDef.GetPawnKindDefs();

        if (pawnKindDefs?.Count > 0)
        {
            var result = new HashSet<ThingDef>();

            foreach (var pawnKindDef in pawnKindDefs)
            {
                if (pawnKindDef.weaponTags != null)
                {
                    foreach (var tag in pawnKindDef.weaponTags)
                    {
                        WeaponsByTag.TryGetValue(tag, out var weapons);

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
}

public static class VerseRecipeDefExtensions
{
    private static readonly Dictionary<RecipeDef, HashSet<ThingDef>> RecipeUsers = [];
    static VerseRecipeDefExtensions()
    {
        foreach (var recipeDef in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            if (recipeDef.recipeUsers?.Count > 0)
            {
                var recipeUsersEntryExists = RecipeUsers.TryGetValue(recipeDef, out var recipeUsers);

                if (recipeUsersEntryExists)
                {
                    recipeUsers.AddRange(recipeDef.recipeUsers);
                }
                else
                {
                    RecipeUsers[recipeDef] = [.. recipeDef.recipeUsers];
                }
            }
        }

        foreach (var thingDef in DefDatabase<ThingDef>.AllDefsListForReading)
        {
            if (thingDef.recipes?.Count > 0)
            {
                foreach (var recipeDef in thingDef.recipes)
                {
                    var recipeUsersEntryExists = RecipeUsers.TryGetValue(recipeDef, out var recipeUsers);

                    if (recipeUsersEntryExists)
                    {
                        recipeUsers.Add(thingDef);
                    }
                    else
                    {
                        RecipeUsers[recipeDef] = [thingDef];
                    }
                }
            }
        }
    }
    public static HashSet<ThingDef>? GetAllRecipeUsers(this RecipeDef recipeDef)
    {
        RecipeUsers.TryGetValue(recipeDef, out var recipeUsers);

        return recipeUsers;
    }
}

public static class RimWorldCompProperties_EggLayerExtensions
{
    public static ThingDef GetAnyEggDef(this CompProperties_EggLayer compProps)
    {
        return compProps.eggUnfertilizedDef ?? compProps.eggFertilizedDef;
    }
}
