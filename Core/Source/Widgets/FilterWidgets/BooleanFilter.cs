using System;
using UnityEngine;
using Verse;

namespace Stats.Widgets;

internal sealed class BooleanFilter<TObject> : FilterWidget<TObject, bool, bool?>
{
    public override bool IsActive => Rhs != null;
    public BooleanFilter(Func<TObject, bool> lhs) : base(lhs, null, Operators.IsEqualTo.Instance)
    {
    }
    protected override Vector2 CalcSize()
    {
        return new Vector2(Text.LineHeight * 2f, Text.LineHeight);
    }
    public override void Draw(Rect rect, Vector2 containerSize)
    {
        var origGUIColor = GUI.color;

        if (Rhs != true)
        {
            GUI.color = Globals.GUI.TextColorSecondary;
        }

        if (Widgets.Draw.ButtonImageSubtle(rect.CutByX(rect.width / 2f), Verse.Widgets.CheckboxOnTex))
        {
            Rhs = Rhs == true ? null : true;
        }

        GUI.color = origGUIColor;

        if (Rhs != false)
        {
            GUI.color = Globals.GUI.TextColorSecondary;
        }

        if (Widgets.Draw.ButtonImageSubtle(rect, Verse.Widgets.CheckboxOffTex))
        {
            Rhs = Rhs == false ? null : false;
        }

        GUI.color = origGUIColor;
    }
    public override void Reset()
    {
        base.Reset();

        Rhs = null;
    }

    private static class Operators
    {
        public sealed class IsEqualTo : AbsOperator
        {
            private IsEqualTo() : base("==") { }
            public override bool Eval(bool lhs, bool? rhs) => lhs == rhs;
            public static IsEqualTo Instance { get; } = new();
        }
    }
}
