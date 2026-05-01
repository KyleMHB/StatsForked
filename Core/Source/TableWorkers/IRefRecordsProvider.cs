using System.Collections.Generic;

namespace Stats.TableWorkers;

public interface IRefRecordsProvider<T>
{
    public IEnumerable<T> Records { get; }
}
