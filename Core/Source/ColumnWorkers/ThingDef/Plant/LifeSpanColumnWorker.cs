using RimWorld;
using Stats.TableCells;
using Stats.TableWorkers;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class LifeSpanColumnWorker(ColumnDef columnDef) : StaticColumnWorker<DefBasedObject, NumberTableCell>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.LifespanDays > 0f)
            {
                decimal cellValue = plantProps.LifespanDays.ToDecimal(1);

                return new NumberTableCell(cellValue, "0.0 d");
            }
        }

        return default;
    }
    //public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker) => NumberTableCell.GetDescriptor(columnDef);
}
