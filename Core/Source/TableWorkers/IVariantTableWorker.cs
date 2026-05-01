using System.Collections.Generic;

namespace Stats.TableWorkers;

public interface IVariantTableWorker<TObject>
{
    bool SupportsVariants { get; }
    bool ShowVariantsByDefault { get; }
    List<TObject> GetObjects(bool showVariants);
}
