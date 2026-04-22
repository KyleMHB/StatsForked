using System;
using System.Collections.Generic;

namespace Stats.Filters;

public sealed class OTMFilter<TOption> : NTMFilter<TOption, TOption>
{
    public OTMFilter(
        Func<int, TOption> cellValueFunc,
        IEnumerable<NTMFilterOption<TOption>> options,
        string? label = null
    ) : base(
        cellValueFunc,
        options,
        [
            Operators.IsIn.Instance,
            Operators.IsNotIn.Instance
        ],
        Operators.IsIn.Instance,
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
