using System;
using System.Collections.Generic;
using System.Linq;
using Stats.ColumnWorkers;
using Verse;

namespace Stats.TableWorkers;

public abstract class TableWorker
{
    public TableDef Def { get; }
    internal abstract ObjectTable TableWidget { get; }

    protected TableWorker(TableDef def)
    {
        Def = def;
    }
}

public abstract class TableWorker<TObject> : TableWorker
{
    internal sealed override ObjectTable TableWidget => new ObjectTable<TObject>(this);
    internal readonly List<ColumnDef> CompatibleColumns;
    public abstract List<TObject> InitialObjects { get; }

    public abstract event Action<TObject> OnObjectAdded;
    public abstract event Action<TObject> OnObjectRemoved;

    protected TableWorker(TableDef def) : base(def)
    {
        List<ColumnDef> compatibleColumns = [];
        List<ColumnDef> columnDefs = DefDatabase<ColumnDef>.AllDefsListForReading;
        int columnDefsCount = columnDefs.Count;
        for (int i = 0; i < columnDefsCount; i++)
        {
            ColumnDef columnDef = columnDefs[i];
            Type workerClass = columnDef.workerClass;
            if (
                typeof(ColumnWorker<TObject>).IsAssignableFrom(workerClass)
                && columnDef.tags.Count != 0
                && columnDef.tags.All(Def.columnTags.Contains)// Is table's column tags is superset of column's tags.
            )
            {
                compatibleColumns.Add(columnDef);
            }
        }
        CompatibleColumns = compatibleColumns;
    }
}
