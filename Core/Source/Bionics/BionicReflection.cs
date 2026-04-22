using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using HarmonyLib;
using RimWorld;
using Verse;

namespace Stats.Bionics;

internal static class BionicReflection
{
    private static readonly FieldInfo? RecipeAddsHediffField = AccessTools.Field(typeof(RecipeDef), "addsHediff");
    private static readonly FieldInfo? RecipeAppliedOnFixedBodyPartsField = AccessTools.Field(typeof(RecipeDef), "appliedOnFixedBodyParts");
    private static readonly FieldInfo? HediffAddedPartPropsField = AccessTools.Field(typeof(HediffDef), "addedPartProps");
    private static readonly FieldInfo? HediffSpawnThingOnRemovedField = AccessTools.Field(typeof(HediffDef), "spawnThingOnRemoved");
    private static readonly FieldInfo? HediffStagesField = AccessTools.Field(typeof(HediffDef), "stages");
    private static readonly FieldInfo? HediffAbilitiesField = AccessTools.Field(typeof(HediffDef), "abilities");
    private static readonly FieldInfo? HediffPreventsLungRotField = AccessTools.Field(typeof(HediffDef), "preventsLungRot");

    private static readonly IReadOnlyDictionary<string, string> EffectLabels = new Dictionary<string, string>
    {
        [BionicEffectKeys.Consciousness] = "Consciousness",
        [BionicEffectKeys.Moving] = "Moving",
        [BionicEffectKeys.Manipulation] = "Manipulation",
        [BionicEffectKeys.Talking] = "Talking",
        [BionicEffectKeys.Sight] = "Sight",
        [BionicEffectKeys.Hearing] = "Hearing",
        [BionicEffectKeys.Breathing] = "Breathing",
        [BionicEffectKeys.BloodFiltration] = "Blood filtration",
        [BionicEffectKeys.BloodPumping] = "Blood pumping",
        [BionicEffectKeys.Metabolism] = "Digestion",
        [BionicEffectKeys.Beauty] = "Beauty"
    };

    private static readonly IReadOnlyList<string> EffectOrder =
    [
        BionicEffectKeys.Consciousness,
        BionicEffectKeys.Moving,
        BionicEffectKeys.Manipulation,
        BionicEffectKeys.Talking,
        BionicEffectKeys.Sight,
        BionicEffectKeys.Hearing,
        BionicEffectKeys.Breathing,
        BionicEffectKeys.BloodFiltration,
        BionicEffectKeys.BloodPumping,
        BionicEffectKeys.Metabolism,
        BionicEffectKeys.Beauty
    ];

    public static HediffDef? GetAddedHediff(RecipeDef recipe)
    {
        return RecipeAddsHediffField?.GetValue(recipe) as HediffDef;
    }

    public static IReadOnlyCollection<BodyPartDef> GetFixedBodyParts(RecipeDef recipe)
    {
        if (RecipeAppliedOnFixedBodyPartsField?.GetValue(recipe) is IEnumerable bodyParts)
        {
            return bodyParts.Cast<BodyPartDef>().ToArray();
        }

        return [];
    }

    public static ThingDef? GetLinkedThingDef(HediffDef hediffDef)
    {
        return HediffSpawnThingOnRemovedField?.GetValue(hediffDef) as ThingDef;
    }

    public static string GetDisplayLabel(RecipeDef recipe, HediffDef hediffDef)
    {
        return GetLinkedThingDef(hediffDef)?.LabelCap.RawText
            ?? hediffDef.LabelCap.RawText
            ?? recipe.LabelCap.RawText;
    }

    public static bool IsBionicOperation(RecipeDef recipe, HediffDef hediffDef)
    {
        IReadOnlyCollection<BodyPartDef> bodyParts = GetFixedBodyParts(recipe);
        (IReadOnlyList<BionicEffectValue> effects, IReadOnlyList<string> specialEffects) = GetEffects(hediffDef, bodyParts);
        return bodyParts.Count > 0
            || GetEfficiency(hediffDef) > 0m
            || GetAffectedCapacities(hediffDef, bodyParts).Count > 0
            || effects.Count > 0
            || specialEffects.Count > 0;
    }

    public static decimal GetEfficiency(HediffDef hediffDef)
    {
        object? addedPartProps = HediffAddedPartPropsField?.GetValue(hediffDef);
        if (addedPartProps == null)
        {
            return 0m;
        }

        FieldInfo? partEfficiencyField = AccessTools.Field(addedPartProps.GetType(), "partEfficiency");
        return partEfficiencyField?.GetValue(addedPartProps) is float partEfficiency
            ? (decimal)partEfficiency
            : 0m;
    }

    public static IReadOnlyCollection<PawnCapacityDef> GetAffectedCapacities(HediffDef hediffDef, IReadOnlyCollection<BodyPartDef> bodyParts)
    {
        HashSet<PawnCapacityDef> result = [];

        foreach (BodyPartDef bodyPart in bodyParts)
        {
            string? effectKey = GetCapacityEffectKey(bodyPart);
            PawnCapacityDef? capacityDef = GetCapacityDef(effectKey);
            if (capacityDef != null)
            {
                result.Add(capacityDef);
            }
        }

        foreach (object stage in GetStages(hediffDef))
        {
            if (GetFieldValue(stage, "capMods") is not IEnumerable capMods)
            {
                continue;
            }

            foreach (object capMod in capMods)
            {
                if (GetFieldValue(capMod, "capacity") is PawnCapacityDef capacityDef)
                {
                    result.Add(capacityDef);
                }
            }
        }

        return result;
    }

    public static (IReadOnlyList<BionicEffectValue> effects, IReadOnlyList<string> specialEffects) GetEffects(HediffDef hediffDef, IReadOnlyCollection<BodyPartDef> bodyParts)
    {
        Dictionary<string, BionicEffectValue> effects = [];
        HashSet<string> specialEffects = [];

        decimal efficiency = GetEfficiency(hediffDef);
        if (efficiency != 0m)
        {
            AddDerivedBodyPartEffects(effects, bodyParts, (efficiency - 1m) * 100m);
        }

        foreach (object stage in GetStages(hediffDef))
        {
            decimal partEfficiencyOffset = GetDecimalField(stage, "partEfficiencyOffset") * 100m;
            if (partEfficiencyOffset != 0m)
            {
                AddDerivedBodyPartEffects(effects, bodyParts, partEfficiencyOffset);
            }

            if (GetFieldValue(stage, "capMods") is IEnumerable capMods)
            {
                foreach (object capMod in capMods)
                {
                    if (GetFieldValue(capMod, "capacity") is not PawnCapacityDef capacityDef)
                    {
                        continue;
                    }

                    string effectKey = capacityDef.defName;
                    string effectLabel = EffectLabels.TryGetValue(effectKey, out string? label)
                        ? label
                        : capacityDef.LabelCap.RawText;
                    AddEffect(effects, effectKey, effectLabel, GetDecimalField(capMod, "offset") * 100m, isPercent: true);
                }
            }

            if (GetFieldValue(stage, "statOffsets") is IEnumerable statOffsets)
            {
                foreach (object statOffset in statOffsets)
                {
                    if (GetFieldValue(statOffset, "stat") is not StatDef statDef)
                    {
                        continue;
                    }

                    decimal value = GetDecimalField(statOffset, "value");
                    if (value == 0m)
                    {
                        continue;
                    }

                    if (statDef.defName == "PawnBeauty")
                    {
                        AddEffect(effects, BionicEffectKeys.Beauty, EffectLabels[BionicEffectKeys.Beauty], value, isPercent: false);
                        continue;
                    }

                    specialEffects.Add($"{statDef.LabelCap.RawText} {FormatSignedValue(value)}");
                }
            }

            if (GetFieldValue(stage, "statFactors") is IEnumerable statFactors)
            {
                foreach (object statFactor in statFactors)
                {
                    if (GetFieldValue(statFactor, "stat") is not StatDef statDef)
                    {
                        continue;
                    }

                    decimal value = GetDecimalField(statFactor, "value");
                    if (value != 0m && value != 1m)
                    {
                        specialEffects.Add($"{statDef.LabelCap.RawText} x{value:0.##}");
                    }
                }
            }

            decimal painFactor = GetDecimalField(stage, "painFactor");
            if (painFactor != 0m)
            {
                specialEffects.Add($"Pain x{painFactor:0.##}");
            }

            decimal painOffset = GetDecimalField(stage, "painOffset") * 100m;
            if (painOffset != 0m)
            {
                specialEffects.Add($"Pain {painOffset:+0;-0;0}%");
            }

            decimal foodPoisoningChanceFactor = GetDecimalField(stage, "foodPoisoningChanceFactor");
            if (foodPoisoningChanceFactor == 0m && FieldExists(stage, "foodPoisoningChanceFactor"))
            {
                specialEffects.Add("Food poison immune");
            }
            else if (foodPoisoningChanceFactor != 0m && foodPoisoningChanceFactor != 1m)
            {
                specialEffects.Add($"Food poisoning x{foodPoisoningChanceFactor:0.##}");
            }

            decimal hungerRateFactor = GetDecimalField(stage, "hungerRateFactor");
            if (hungerRateFactor != 0m && hungerRateFactor != 1m)
            {
                specialEffects.Add($"Hunger rate x{hungerRateFactor:0.##}");
            }

            decimal hungerRateFactorOffset = GetDecimalField(stage, "hungerRateFactorOffset") * 100m;
            if (hungerRateFactorOffset != 0m)
            {
                specialEffects.Add($"Hunger rate {hungerRateFactorOffset:+0;-0;0}%");
            }

            if (GetFieldValue(stage, "makeImmuneTo") is IEnumerable immuneHediffs)
            {
                foreach (object immuneHediff in immuneHediffs)
                {
                    if (immuneHediff is HediffDef immuneDef)
                    {
                        specialEffects.Add($"Immune: {immuneDef.LabelCap.RawText}");
                    }
                }
            }
        }

        if (HediffPreventsLungRotField?.GetValue(hediffDef) is true)
        {
            specialEffects.Add("Prevents lung rot");
        }

        if (HediffAbilitiesField?.GetValue(hediffDef) is IEnumerable abilities)
        {
            foreach (object ability in abilities)
            {
                if (ability is Def abilityDef)
                {
                    specialEffects.Add($"Ability: {abilityDef.LabelCap.RawText}");
                }
            }
        }

        return (
            effects.Values.OrderBy(effect => GetEffectOrder(effect.Key)).ThenBy(effect => effect.Label).ToArray(),
            specialEffects.OrderBy(text => text).ToArray());
    }

    private static IEnumerable<object> GetStages(HediffDef hediffDef)
    {
        object? stages = HediffStagesField?.GetValue(hediffDef);
        return stages is IEnumerable enumerable ? enumerable.Cast<object>() : [];
    }

    private static void AddDerivedBodyPartEffects(Dictionary<string, BionicEffectValue> effects, IReadOnlyCollection<BodyPartDef> bodyParts, decimal value)
    {
        if (value == 0m)
        {
            return;
        }

        foreach (BodyPartDef bodyPart in bodyParts)
        {
            string? effectKey = GetCapacityEffectKey(bodyPart);
            if (effectKey == null || EffectLabels.TryGetValue(effectKey, out string? effectLabel) == false)
            {
                continue;
            }

            AddEffect(effects, effectKey, effectLabel, value, isPercent: true);
        }
    }

    private static void AddEffect(Dictionary<string, BionicEffectValue> effects, string key, string label, decimal value, bool isPercent)
    {
        if (value == 0m)
        {
            return;
        }

        if (effects.TryGetValue(key, out BionicEffectValue? existing))
        {
            effects[key] = new BionicEffectValue(key, label, existing.Value + value, isPercent);
        }
        else
        {
            effects[key] = new BionicEffectValue(key, label, value, isPercent);
        }
    }

    private static string? GetCapacityEffectKey(BodyPartDef bodyPart)
    {
        string name = bodyPart.defName.ToLowerInvariant();
        return name switch
        {
            var value when value.Contains("heart") => BionicEffectKeys.BloodPumping,
            var value when value.Contains("lung") => BionicEffectKeys.Breathing,
            var value when value.Contains("kidney") || value.Contains("liver") => BionicEffectKeys.BloodFiltration,
            var value when value.Contains("stomach") => BionicEffectKeys.Metabolism,
            var value when value.Contains("eye") => BionicEffectKeys.Sight,
            var value when value.Contains("ear") => BionicEffectKeys.Hearing,
            var value when value.Contains("jaw") || value.Contains("tongue") => BionicEffectKeys.Talking,
            var value when value.Contains("brain") => BionicEffectKeys.Consciousness,
            var value when value.Contains("arm") || value.Contains("hand") || value.Contains("finger") || value.Contains("claw") || value.Contains("shoulder") => BionicEffectKeys.Manipulation,
            var value when value.Contains("leg") || value.Contains("foot") || value.Contains("toe") || value.Contains("spine") => BionicEffectKeys.Moving,
            _ => null
        };
    }

    private static PawnCapacityDef? GetCapacityDef(string? effectKey)
    {
        if (effectKey == null || effectKey == BionicEffectKeys.Beauty)
        {
            return null;
        }

        return DefDatabase<PawnCapacityDef>.GetNamedSilentFail(effectKey);
    }

    private static int GetEffectOrder(string key)
    {
        int index = EffectOrder.ToList().IndexOf(key);
        return index >= 0 ? index : int.MaxValue;
    }

    private static object? GetFieldValue(object instance, string fieldName)
    {
        return AccessTools.Field(instance.GetType(), fieldName)?.GetValue(instance);
    }

    private static decimal GetDecimalField(object instance, string fieldName)
    {
        object? value = GetFieldValue(instance, fieldName);
        return value switch
        {
            null => 0m,
            float floatValue => (decimal)floatValue,
            double doubleValue => (decimal)doubleValue,
            int intValue => intValue,
            decimal decimalValue => decimalValue,
            _ => 0m
        };
    }

    private static bool FieldExists(object instance, string fieldName)
    {
        return AccessTools.Field(instance.GetType(), fieldName) != null;
    }

    private static string FormatSignedValue(decimal value)
    {
        return value.ToString("+0.##;-0.##;0");
    }
}

