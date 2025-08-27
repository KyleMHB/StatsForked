using System;
using System.Collections.Generic;

namespace Stats.Widgets;

// TODO: We don't need this generic parameter anymore.
public abstract class FilterWidget : Widget
{
    public abstract bool IsActive { get; }
    public abstract event Action<FilterWidget>? OnChange;
    public abstract bool Eval(Dictionary<ColumnWorker, ObjectTable.Cell> cells);
    public abstract void Reset();
    public abstract void NotifyChanged();

    protected abstract class RelOperator<TLhs, TRhs>
    {
        public string Symbol { get; }
        public string Description { get; }
        protected RelOperator(string symbol = "", string description = "")
        {
            Symbol = symbol;
            Description = description;
        }
        public abstract bool Eval(TLhs lhs, TRhs rhs);
    }
}
