using System;
using Stats.ObjectTable.Cells;
using Stats.Widgets;

namespace Stats.ObjectTable.FilterWidgets;

public abstract class FilterWidget : Widget
{
    public abstract bool IsActive { get; }
    public abstract event Action? OnChange;
    public abstract bool Eval(Cell cell);
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
