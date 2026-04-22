using System.Collections;
using System.Collections.Generic;
using System.Linq;
using HarmonyLib;
using Verse;

namespace Stats.Bionics;

internal static class BionicReflection
{
    private static readonly System.Reflection.FieldInfo? _recipeAddsHediffField = AccessTools.Field(typeof(RecipeDef), "addsHediff");
    private static readonly System.Reflection.FieldInfo? _recipeAppliedOnFixedBodyPartsField = AccessTools.Field(typeof(RecipeDef), "appliedOnFixedBodyParts");
    private static readonly System.Reflection.FieldInfo? _hediffAddedPartPropsField = AccessTools.Field(typeof(HediffDef), "addedPartProps");
    private static readonly System.Reflection.FieldInfo? _hediffStagesField = AccessTools.Field(typeof(HediffDef), "stages");
    private static readonly System.Reflection.FieldInfo? _addedPartEfficiencyField = AccessTools.Field(typeof(HediffDef), "addedPartProps.partEfficiency");

    public static HediffDef? GetAddedHediff(RecipeDef recipe)
    {
        return _recipeAddsHediffField?.GetValue(recipe) as HediffDef;
    }

    public static IReadOnlyCollection<BodyPartDef> GetFixedBodyParts(RecipeDef recipe)
    {
        if (_recipeAppliedOnFixedBodyPartsField?.GetValue(recipe) is IEnumerable bodyParts)
        {
            return bodyParts.Cast<BodyPartDef>().ToArray();
        }

        return [];
    }

    public static bool IsBionicOperation(RecipeDef recipe, HediffDef hediffDef)
    {
        return GetFixedBodyParts(recipe).Count > 0
            || GetEfficiency(hediffDef) > 0m
            || GetAffectedCapacities(hediffDef).Count > 0;
    }

    public static decimal GetEfficiency(HediffDef hediffDef)
    {
        object? addedPartProps = _hediffAddedPartPropsField?.GetValue(hediffDef);
        if (addedPartProps != null)
        {
            System.Reflection.FieldInfo? partEfficiencyField = AccessTools.Field(addedPartProps.GetType(), "partEfficiency");
            if (partEfficiencyField?.GetValue(addedPartProps) is float partEfficiency)
            {
                return (decimal)partEfficiency;
            }
        }

        return 0m;
    }

    public static IReadOnlyCollection<PawnCapacityDef> GetAffectedCapacities(HediffDef hediffDef)
    {
        HashSet<PawnCapacityDef> result = [];
        if (_hediffStagesField?.GetValue(hediffDef) is not IEnumerable stages)
        {
            return result;
        }

        foreach (object stage in stages)
        {
            System.Reflection.FieldInfo? capModsField = AccessTools.Field(stage.GetType(), "capMods");
            if (capModsField?.GetValue(stage) is not IEnumerable capMods)
            {
                continue;
            }

            foreach (object capMod in capMods)
            {
                System.Reflection.FieldInfo? capacityField = AccessTools.Field(capMod.GetType(), "capacity");
                if (capacityField?.GetValue(capMod) is PawnCapacityDef capacityDef)
                {
                    result.Add(capacityDef);
                }
            }
        }

        return result;
    }
}
