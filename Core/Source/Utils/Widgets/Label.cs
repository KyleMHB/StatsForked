using UnityEngine;

namespace Stats.Utils.Widgets;

public sealed class Label : Widget
{
    public override Vector2 Size { get; }

    private readonly string _text;
    private readonly GUIStyle _style;

    // TODO: Pass style explicitly at call sites and remove this constructor.
    public Label(string text) : this(text, GUIStyles.TableCell.StringNoPad) { }

    public Label(string text, GUIStyle style)
    {
        _text = text;
        _style = style;
        Size = text.CalcSize(style);
    }

    public override void Draw(Rect rect)
    {
        rect.Label(_text, _style);
    }
}
