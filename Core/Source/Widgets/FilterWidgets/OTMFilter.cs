using System;
using System.Collections.Generic;

namespace Stats.Widgets;

internal sealed class OTMFilter<TObject, TOption> : NTMFilter<TObject, TOption, TOption>
{
    public OTMFilter(
        Func<TObject, TOption> objectValueFunc,
        IEnumerable<NTMFilterOption<TOption>> options,
        string? label = null
    ) : base(
        objectValueFunc,
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
            private IsIn() : base("∈", "Is one of selected") { }
            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs);
            public static IsIn Instance { get; } = new();
        }

        // ∉
        public sealed class IsNotIn : RelOperator<TOption, HashSet<TOption>>
        {
            private IsNotIn() : base("!∈", "Is not one of selected") { }
            public override bool Eval(TOption lhs, HashSet<TOption> rhs) => rhs.Contains(lhs) == false;
            public static IsNotIn Instance { get; } = new();
        }
    }
}
