using RimWorld;
using Stats.TableCells;

namespace Stats.ColumnWorkers.ThingDef.Plant;

public sealed class FertilitySensitivityColumnWorker(ColumnDef columnDef) : NumberColumnWorker<DefBasedObject>
{
    public override ColumnDef Def => columnDef;

    protected override NumberTableCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            PlantProperties? plantProps = thingDef.plant;

            if (plantProps?.fertilitySensitivity > 0f)
            {
                decimal cellValue = (100f * plantProps.fertilitySensitivity).ToDecimal(0);

                return new NumberTableCell(cellValue, "0\\%");
            }
        }

        return default;
    }
}
