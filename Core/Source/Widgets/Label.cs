using UnityEngine;

namespace Stats.Widgets;

public sealed class Label : Widget
{
    private string _Text;
    public string Text
    {
        get => _Text;
        set
        {
            if (value == _Text)
            {
                return;
            }

            _Text = value;

            Resize();
        }
    }
    public Label(string text)
    {
        _Text = text;
    }
    internal Label(Observable<string> observable)
    {
        _Text = observable.Value;
        observable.OnNext += value => Text = value;
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

        Verse.Widgets.Label(rect, Text);
    }
}
