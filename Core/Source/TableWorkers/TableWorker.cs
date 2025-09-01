using System;
using System.Collections.Generic;
using Stats.Widgets;

namespace Stats;

public abstract class TableWorker
{
    public TableDef TableDef { get; }
    internal abstract ObjectTable TableWidget { get; }
    protected TableWorker(TableDef tableDef)
    {
        TableDef = tableDef;
    }

    public interface IReferenceObjectsProvider<TObject>
    {
        public IEnumerable<TObject> ReferenceObjects { get; }
    }
}

public abstract class TableWorker<TObject> : TableWorker
{
    // We don't want to create every table widget on the start of the game.
    internal sealed override ObjectTable TableWidget => field ??= new ObjectTable<TObject>(this);
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
