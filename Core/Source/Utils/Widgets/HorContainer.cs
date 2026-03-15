using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats.Utils.Widgets;

public sealed class HorContainer : Widget
{
    public override Vector2 Size { get; }

    private readonly Widget[] _widgets;
    private readonly float _gap;

    public HorContainer(Widget[] widgets, float gap = 0f)
    {
        _widgets = widgets;
        _gap = gap;
        Vector2 size = Vector2.zero;

        int widgetsCount = widgets.Length;
        for (int i = 0; i < widgetsCount; i++)
        {
            Widget widget = widgets[i];
            Vector2 widgetSize = widget.Size;

            size.x += widgetSize.x;
            if (size.y < widgetSize.y)
            {
                size.y = widgetSize.y;
            }
        }

        size.x += (widgets.Length - 1) * gap;
        Size = size;
    }

    public override void Draw(Rect rect)
    {
        float gap = _gap;
        int widgetsCount = _widgets.Length;
        for (int i = 0; i < widgetsCount; i++)
        {
            Widget widget = _widgets[i];
            float widgetWidth = widget.Size.x;
            Rect widgetRect = rect.CutByX(widgetWidth);
            rect.xMin += gap;
            widget.Draw(widgetRect);
        }
    }
}
