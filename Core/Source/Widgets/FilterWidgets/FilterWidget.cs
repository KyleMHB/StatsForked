using System;

namespace Stats.Widgets;

public abstract class FilterWidget<TObject> : Widget
{
    public abstract bool IsActive { get; }
    public abstract event Action<FilterWidget<TObject>>? OnChange;
    public abstract bool Eval(TObject @object);
    public abstract void Reset();

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
