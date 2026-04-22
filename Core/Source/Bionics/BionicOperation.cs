using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using Verse;

namespace Stats.Bionics;

public static class BionicEffectKeys
{
    public const string Consciousness = nameof(Consciousness);
    public const string Moving = nameof(Moving);
    public const string Manipulation = nameof(Manipulation);
    public const string Talking = nameof(Talking);
    public const string Sight = nameof(Sight);
    public const string Hearing = nameof(Hearing);
    public const string Breathing = nameof(Breathing);
    public const string BloodFiltration = nameof(BloodFiltration);
    public const string BloodPumping = nameof(BloodPumping);
    public const string Metabolism = nameof(Metabolism);
    public const string Beauty = nameof(Beauty);
}

public sealed class BionicEffectValue
{
    public string Key { get; }
    public string Label { get; }
    public decimal Value { get; }
    public bool IsPercent { get; }
    public string DisplayText => IsPercent
        ? $"{Value:+0;-0;0}%"
        : $"{Value:+0.##;-0.##;0}";

    public BionicEffectValue(string key, string label, decimal value, bool isPercent)
    {
        Key = key;
        Label = label;
        Value = value;
        IsPercent = isPercent;
    }
}

public sealed class BionicOperation
{
    private readonly Dictionary<string, BionicEffectValue> _effectsByKey;

    public RecipeDef Recipe { get; }
    public HediffDef HediffDef { get; }
    public ThingDef? ThingDef { get; }
    public string DisplayLabel { get; }
    public IReadOnlyCollection<BodyPartDef> BodyParts { get; }
    public IReadOnlyCollection<PawnCapacityDef> Capacities { get; }
    public decimal Efficiency { get; }
    public IReadOnlyList<BionicEffectValue> Effects { get; }
    public IReadOnlyList<string> SpecialEffects { get; }
    public string? EffectsSummary { get; }

    public BionicOperation(
        RecipeDef recipe,
        HediffDef hediffDef,
        ThingDef? thingDef,
        string displayLabel,
        IReadOnlyCollection<BodyPartDef> bodyParts,
        IReadOnlyCollection<PawnCapacityDef> capacities,
        decimal efficiency,
        IReadOnlyList<BionicEffectValue> effects,
        IReadOnlyList<string> specialEffects)
    {
        Recipe = recipe;
        HediffDef = hediffDef;
        ThingDef = thingDef;
        DisplayLabel = displayLabel;
        BodyParts = bodyParts;
        Capacities = capacities;
        Efficiency = efficiency;
        Effects = effects;
        SpecialEffects = specialEffects;
        _effectsByKey = effects.ToDictionary(effect => effect.Key);

        List<string> summaryParts = effects.Select(effect => $"{effect.Label} {effect.DisplayText}").ToList();
        summaryParts.AddRange(specialEffects);
        EffectsSummary = summaryParts.Count == 0 ? null : string.Join(", ", summaryParts);
    }

    public bool TryGetEffect(string key, [NotNullWhen(true)] out BionicEffectValue? effect)
    {
        return _effectsByKey.TryGetValue(key, out effect);
    }
}
