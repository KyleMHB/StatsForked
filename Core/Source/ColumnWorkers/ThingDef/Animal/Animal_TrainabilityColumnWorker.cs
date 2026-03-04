using System.Collections.Generic;
using System.Linq;
using Stats.TableCells;
using Stats.TableWorkers;
using Verse;

namespace Stats.ColumnWorkers.ThingDef.Animal;

public sealed class Animal_TrainabilityColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject,>
{
    public override ColumnDef Def => columnDef;

    public override Cell MakeCell(Verse.ThingDef thingDef)
    {
        TrainabilityDef? trainability = thingDef.race?.trainability;

        if (trainability != null)
        {
            return new DefTableCell.Constant(trainability);
        }

        return DefTableCell.Empty;
    }
    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<TrainabilityDef?> trainabilityDefs = ((IRefRecordsProvider)tableWorker).Records
            .Select(thingDef => thingDef.race?.trainability)
            .Distinct();

        return DefTableCell.GetDescriptor(columnDef, trainabilityDefs);
    }
}
