using System.Collections.Generic;
using Verse;

namespace Stats.Bionics;

public sealed class BionicOperation
{
    public RecipeDef Recipe { get; }
    public HediffDef HediffDef { get; }
    public IReadOnlyCollection<BodyPartDef> BodyParts { get; }
    public IReadOnlyCollection<PawnCapacityDef> Capacities { get; }
    public decimal Efficiency { get; }

    public BionicOperation(
        RecipeDef recipe,
        HediffDef hediffDef,
        IReadOnlyCollection<BodyPartDef> bodyParts,
        IReadOnlyCollection<PawnCapacityDef> capacities,
        decimal efficiency)
    {
        Recipe = recipe;
        HediffDef = hediffDef;
        BodyParts = bodyParts;
        Capacities = capacities;
        Efficiency = efficiency;
    }
}
