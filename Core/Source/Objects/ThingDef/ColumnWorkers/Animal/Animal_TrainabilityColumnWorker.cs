using System.Collections.Generic;
using System.Linq;
using Stats.Objects.ThingDef.TableWorkers;
using Stats.ObjectTable;
using Stats.ObjectTable.Cells;
using Verse;

namespace Stats.Objects.ThingDef.ColumnWorkers.Animal;

public sealed class Animal_TrainabilityColumnWorker(ColumnDef columnDef) : ThingDefColumnWorker
{
    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        TrainabilityDef? trainability = thingDef.race?.trainability;

        if (trainability != null)
        {
            return new DefCell(trainability);
        }

        return DefCell.Empty;
    }
    public override CellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<TrainabilityDef?> trainabilityDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.race?.trainability)
            .Distinct();

        return DefCell.GetDescriptor(columnDef, trainabilityDefs);
    }
}
