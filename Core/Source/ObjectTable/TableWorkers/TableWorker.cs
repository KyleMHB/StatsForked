using System;
using System.Collections.Generic;
using Stats.ObjectTable;

namespace Stats.ObjectTable.TableWorkers;

public abstract class TableWorker
{
    public TableDef TableDef { get; }
    internal abstract ObjectTableWidget TableWidget { get; }
    protected TableWorker(TableDef tableDef)
    {
        TableDef = tableDef;
    }
}

public abstract class TableWorker<TObject> : TableWorker
{
    // We don't want to create every table widget on the start of the game.
    internal sealed override ObjectTableWidget TableWidget => field ??= new ObjectTableWidget<TObject>(this);
    public abstract IEnumerable<TObject> InitialObjects { get; }
    protected TableWorker(TableDef tableDef) : base(tableDef)
    {
    }

    public interface IStreaming
    {
        public event Action<TObject> OnObjectAdded;
        public event Action<TObject> OnObjectRemoved;
    }
}
