using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.Filters;

public sealed class MTMFilter<TOption> : NTMFilter<IEnumerable<TOption>, TOption>
{
    public override bool IsActive =>
        base.IsActive
        || Operator == Operators.IsEqualTo.Instance
        || Operator == Operators.IsNotEqualTo.Instance;
    public MTMFilter(
        Func<int, IEnumerable<TOption>> cellValueFunc,
        IEnumerable<NTMFilterOption<TOption>> options,
        string? label = null
    ) : base(
        cellValueFunc,
        options,
        [
            Operators.IntersectsWith.Instance,
            Operators.NotIntersectsWith.Instance,
            Operators.IsSubsetOf.Instance,
            Operators.IsSupersetOf.Instance,
            Operators.IsEqualTo.Instance,
            Operators.IsNotEqualTo.Instance,
        ],
        Operators.IntersectsWith.Instance,
        label
    )
    {
    }

    // Reasons for using "! + operator" instead of proper negated set operator symbols:
    // - It looks like there is no negated form for "∩".
    // - Because of font the game uses, "⊄"/"⊅" are unrecognizable in the game.
    private static class Operators
    {
        public sealed class IsEqualTo : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsEqualTo() : base("==", Localization.Get(Localization.FilterIsEqualTo)) { }
            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.SetEquals(lhs);
            public static IsEqualTo Instance { get; } = new();
        }

        public sealed class IsNotEqualTo : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsNotEqualTo() : base("!=", Localization.Get(Localization.FilterIsNotEqualTo)) { }
            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.SetEquals(lhs) == false;
            public static IsNotEqualTo Instance { get; } = new();
        }

        public sealed class IntersectsWith : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IntersectsWith() : base("∩", Localization.Get(Localization.FilterContainsAtLeastOne)) { }
            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains);
            public static IntersectsWith Instance { get; } = new();
        }

        public sealed class NotIntersectsWith : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private NotIntersectsWith() : base("!∩", Localization.Get(Localization.FilterDoesNotContainAny)) { }
            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains) == false;
            public static NotIntersectsWith Instance { get; } = new();
        }

        public sealed class IsSubsetOf : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsSubsetOf() : base("⊆", Localization.Get(Localization.FilterIsSubsetOf)) { }
            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.IsSupersetOf(lhs);
            public static IsSubsetOf Instance { get; } = new();
        }

        public sealed class IsSupersetOf : RelOperator<IEnumerable<TOption>, HashSet<TOption>>
        {
            private IsSupersetOf() : base("⊇", Localization.Get(Localization.FilterIsSupersetOf)) { }
            public override bool Eval(IEnumerable<TOption> lhs, HashSet<TOption> rhs) => rhs.IsSubsetOf(lhs);
            public static IsSupersetOf Instance { get; } = new();
        }
    }
}
