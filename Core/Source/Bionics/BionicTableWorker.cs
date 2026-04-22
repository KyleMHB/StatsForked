using System;
using System.Collections.Generic;
using System.Linq;
using Stats.TableWorkers;
using Verse;

namespace Stats.Bionics;

public sealed class BionicTableWorker : TableWorker<BionicOperation>
{
    public override List<BionicOperation> InitialObjects { get; }
    public override event Action<BionicOperation>? OnObjectAdded;
    public override event Action<BionicOperation>? OnObjectRemoved;

    public BionicTableWorker(TableDef tableDef) : base(tableDef)
    {
        List<BionicOperation> operations = [];
        foreach (RecipeDef recipe in DefDatabase<RecipeDef>.AllDefsListForReading)
        {
            HediffDef? hediffDef = BionicReflection.GetAddedHediff(recipe);
            if (hediffDef == null || BionicReflection.IsBionicOperation(recipe, hediffDef) == false)
            {
                continue;
            }

            IReadOnlyCollection<BodyPartDef> bodyParts = BionicReflection.GetFixedBodyParts(recipe);
            (IReadOnlyList<BionicEffectValue> effects, IReadOnlyList<string> specialEffects) = BionicReflection.GetEffects(hediffDef, bodyParts);
            operations.Add(new BionicOperation(
                recipe,
                hediffDef,
                BionicReflection.GetLinkedThingDef(hediffDef),
                BionicReflection.GetDisplayLabel(recipe, hediffDef),
                bodyParts,
                BionicReflection.GetAffectedCapacities(hediffDef, bodyParts),
                BionicReflection.GetEfficiency(hediffDef),
                effects,
                specialEffects));
        }

        InitialObjects = operations
            .OrderBy(operation => operation.DisplayLabel)
            .ThenBy(operation => operation.Recipe.defName)
            .ToList();
    }
}
