using System;
using System.Collections.Generic;
using System.Linq;
using Verse;

namespace Stats.Widgets;

internal sealed class MTMFilter<TObject, TOption> : NTMFilter<TObject, HashSet<TOption>, TOption>
{
    public MTMFilter(
        Func<TObject, HashSet<TOption>> objectValueFunc,
        IEnumerable<NTMFilterOption<TOption>> options,
        string? label = null
    ) : base(
        objectValueFunc,
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
        public sealed class IsEqualTo : RelOperator<HashSet<TOption>, HashSet<TOption>>
        {
            private IsEqualTo() : base("==", "Is equal to") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.SetEquals(rhs);
            public static IsEqualTo Instance { get; } = new();
        }

        public sealed class IsNotEqualTo : RelOperator<HashSet<TOption>, HashSet<TOption>>
        {
            private IsNotEqualTo() : base("!=", "Is not equal to") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.SetEquals(rhs) == false;
            public static IsNotEqualTo Instance { get; } = new();
        }

        public sealed class IntersectsWith : RelOperator<HashSet<TOption>, HashSet<TOption>>
        {
            private IntersectsWith() : base("∩", "Contains at least one of") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains);
            public static IntersectsWith Instance { get; } = new();
        }

        public sealed class NotIntersectsWith : RelOperator<HashSet<TOption>, HashSet<TOption>>
        {
            private NotIntersectsWith() : base("!∩", "Does not contain any of") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => rhs.Any(lhs.Contains) == false;
            public static NotIntersectsWith Instance { get; } = new();
        }

        public sealed class IsSubsetOf : RelOperator<HashSet<TOption>, HashSet<TOption>>
        {
            private IsSubsetOf() : base("⊆", "Is subset of") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSubsetOf(rhs);
            public static IsSubsetOf Instance { get; } = new();
        }

        public sealed class IsSupersetOf : RelOperator<HashSet<TOption>, HashSet<TOption>>
        {
            private IsSupersetOf() : base("⊇", "Is superset of") { }
            public override bool Eval(HashSet<TOption> lhs, HashSet<TOption> rhs) => lhs.IsSupersetOf(rhs);
            public static IsSupersetOf Instance { get; } = new();
        }
    }
}
