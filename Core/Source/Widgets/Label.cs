using UnityEngine;

namespace Stats.Widgets;

public sealed class Label : Widget
{
    public string Text
    {
        get => _text;
        set
        {
            if (value == _text)
            {
                return;
            }

            _text = value;

            Resize();
        }
    }

    private string _text;

    public Label(string text)
    {
        _text = text;
    }

    internal Label(Observable<string> observable)
    {
        _text = observable.Value;
        observable.OnNext += value => Text = value;
    }

    public static readonly GUIStyle Style;

    static Label()
    {
        Style = new GUIStyle(Verse.Text.fontStyles[1]);
        Style.alignment = TextAnchor.MiddleLeft;
        Style.wordWrap = false;
    }

    public override Vector2 GetSize()
    {
        return Verse.Text.CalcSize(Text);
    }

    public override void Draw(Rect rect, Vector2 _)
    {
        GUIDebugger.DebugRect(this, rect);

        if (Event.current.type != EventType.Repaint)
        {
            return;
        }

        Widgets.Draw.Label(rect, Text, Style);
    }
}
