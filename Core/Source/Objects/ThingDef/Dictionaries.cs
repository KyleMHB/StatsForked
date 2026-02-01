using System.Collections.Generic;

namespace Stats.Objects.ThingDef.Dictionaries;

public interface IFuelTypesProvider
{
    public IEnumerable<Verse.ThingDef> FuelTypes { get; }
}
