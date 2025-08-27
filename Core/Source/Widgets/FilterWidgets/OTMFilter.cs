using System;
using System.Collections.Generic;

namespace Stats.Widgets;

public sealed class OTMFilter<TCell, TOption> : NTMFilter<TCell, TOption, TOption> where TCell : ObjectTable.Cell
{
    public OTMFilter(
        Func<TCell, TOption> objectValueFunc,
        IEnumerable<NTMFilterOption<TOption>> options,
        ColumnWorker column,
        string? label = null
    ) : base(
        objectValueFunc,
        options,
        [
            Operators.IsIn.Instance,
            Operators.IsNotIn.Instance
        ],
        Operators.IsIn.Instance,
        column,
        label
    )
    {
    }

    private static class Operators
    {
        public sealed class IsIn : RelOperator<TOption, HashSet<TOption>>
        {
            private IsIn() : base("∈", "Is one of") { }
            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs);
            public static IsIn Instance { get; } = new();
        }

        // ∉
        public sealed class IsNotIn : RelOperator<TOption, HashSet<TOption>>
        {
            private IsNotIn() : base("!∈", "Is not one of") { }
            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs) == false;
            public static IsNotIn Instance { get; } = new();
        }
    }
}
