using System;
using System.Collections.Generic;

namespace Stats.ObjectTable;

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
    public abstract event Action<TObject> OnObjectAdded;
    public abstract event Action<TObject> OnObjectRemoved;
    protected TableWorker(TableDef tableDef) : base(tableDef)
    {
    }
}
