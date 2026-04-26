using System;
using System.Collections.Generic;
using Stats.TableWorkers;
using Verse;

namespace Stats.Compat.Biotech;

public sealed class GeneTableWorker(TableDef tableDef) : TableWorker<GeneDef>(tableDef), IRefRecordsProvider<GeneDef>
{
    public override List<GeneDef> InitialObjects { get; } = DefDatabase<GeneDef>.AllDefsListForReading.ListFullCopy();

    IEnumerable<GeneDef> IRefRecordsProvider<GeneDef>.Records => InitialObjects;

    public override event Action<GeneDef>? OnObjectAdded;
    public override event Action<GeneDef>? OnObjectRemoved;
}
