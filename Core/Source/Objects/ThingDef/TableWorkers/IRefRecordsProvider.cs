using System.Collections.Generic;

namespace Stats.Objects.ThingDef.TableWorkers;

public interface IRefRecordsProvider
{
    public IEnumerable<Verse.ThingDef> Records { get; }
}
