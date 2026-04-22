using Stats.ColumnWorkers;
using Stats.ColumnWorkers.Cells;
using Verse;

namespace Stats.Compat.Biotech;

public abstract class GeneNumberColumnWorker(ColumnDef columnDef) : NumberColumnWorker<GeneDef, NumberCell>
{
    public override ColumnDef Def => columnDef;

    protected abstract decimal GetValue(GeneDef geneDef);

    protected override NumberCell MakeCell(GeneDef geneDef)
    {
        decimal value = GetValue(geneDef);
        if (value != 0m)
        {
            return new NumberCell(value);
        }

        return default;
    }
}
