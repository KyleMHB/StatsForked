using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Stats.TableWorkers;
using Verse;

namespace Stats.Compat.Odyssey;

public abstract class OdysseyNamedDefTableWorker : TableWorker<Def>, IRefRecordsProvider<Def>
{
    public override List<Def> InitialObjects { get; }

    IEnumerable<Def> IRefRecordsProvider<Def>.Records => InitialObjects;

    public override event Action<Def>? OnObjectAdded;
    public override event Action<Def>? OnObjectRemoved;

    protected OdysseyNamedDefTableWorker(TableDef tableDef, string defTypeName) : base(tableDef)
    {
        InitialObjects = GetDefs(defTypeName).ToList();
    }

    private static IEnumerable<Def> GetDefs(string defTypeName)
    {
        Type? defType = AppDomain.CurrentDomain
            .GetAssemblies()
            .SelectMany(GetLoadableTypes)
            .FirstOrDefault(type => type.Name == defTypeName && typeof(Def).IsAssignableFrom(type));
        if (defType == null)
        {
            return [];
        }

        Type databaseType = typeof(DefDatabase<>).MakeGenericType(defType);
        PropertyInfo? allDefsListForReadingProperty = databaseType.GetProperty("AllDefsListForReading", BindingFlags.Public | BindingFlags.Static);
        if (allDefsListForReadingProperty?.GetValue(null) is not IEnumerable<Def> defs)
        {
            return [];
        }

        return defs;
    }

    private static IEnumerable<Type> GetLoadableTypes(Assembly assembly)
    {
        try
        {
            return assembly.GetTypes();
        }
        catch (ReflectionTypeLoadException exception)
        {
            return exception.Types.OfType<Type>();
        }
    }
}

public sealed class NegativeFishingOutcomeTableWorker(TableDef tableDef) : OdysseyNamedDefTableWorker(tableDef, "NegativeFishingOutcomeDef");
