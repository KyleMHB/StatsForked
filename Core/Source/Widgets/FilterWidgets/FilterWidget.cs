using System;
using System.Collections.Generic;
using Verse;

namespace Stats.Widgets;

public abstract class FilterWidget<TObject> : Widget
{
    public abstract bool IsActive { get; }
    public abstract event Action<FilterWidget<TObject>>? OnChange;
    public abstract bool Eval(TObject @object);
    public abstract void Reset();
}

internal abstract class FilterWidget<TObject, TExprLhs, TExprRhs> : FilterWidget<TObject>
{
    private readonly Func<TObject, TExprLhs> Lhs;
    private TExprRhs _Rhs;
    protected TExprRhs Rhs
    {
        get => _Rhs;
        set
        {
            if (EqualityComparer<TExprRhs>.Default.Equals(Rhs, value))
            {
                return;
            }

            _Rhs = value;
            OnChange?.Invoke(this);
        }
    }
    private AbsOperator _Operator;
    protected virtual AbsOperator Operator
    {
        get => _Operator;
        set
        {
            if (_Operator == value)
            {
                return;
            }

            _Operator = value;
            OnChange?.Invoke(this);
        }
    }
    private readonly AbsOperator DefaultOperator;
    public sealed override event Action<FilterWidget<TObject>>? OnChange;
    protected FilterWidget(Func<TObject, TExprLhs> lhs, TExprRhs rhs, AbsOperator defaultOperator)
    {
        Lhs = lhs;
        _Rhs = rhs;
        _Operator = DefaultOperator = defaultOperator;
    }
    public sealed override bool Eval(TObject thing)
    {
        try
        {
            return _Operator.Eval(Lhs(thing), _Rhs);
        }
        catch (Exception e)
        {
            Log.Error(e.Message);

            // Evaluate to "true", so the error would be noticeable.
            return true;
        }
    }
    public override void Reset()
    {
        Operator = DefaultOperator;
    }
    protected void NotifyChanged()
    {
        Resize();
        OnChange?.Invoke(this);
    }

    protected abstract class AbsOperator
    {
        public string Symbol { get; }
        public string Description { get; }
        protected AbsOperator(string symbol = "", string description = "")
        {
            Symbol = symbol;
            Description = description;
        }
        public abstract bool Eval(TExprLhs lhs, TExprRhs rhs);
    }
}
