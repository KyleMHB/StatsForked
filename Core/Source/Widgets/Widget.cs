using UnityEngine;

namespace Stats.Widgets;

// TODO: (re)Implement size caching either as an extension
// or directly, where it is required.
public abstract class Widget
{
    // TODO: Parent is only used to propagate "resize" event.
    // So maybe use events instead?
    public Widget? Parent { private get; set; }
    /*
    
    This method is used to calculate widget's size relative to its container size.

    The only argument is the size of its parent container. It is used to calculate relative dimensions of a widget's box.

    var containerSize = new Vector2(100f, 100f);

    var widget = new ExampleWidget()
        .SizeRel(0.5f, 0.25f);

    widget.GetSize(containerSize);// (50, 25)

    */
    public virtual Vector2 GetSize(Vector2 containerSize)
    {
        return GetSize();
    }
    // This method is used to calculate the "absolute" size of a widget,
    // ie. without any relative-size-related extensions.
    public abstract Vector2 GetSize();
    public abstract void Draw(Rect rect, Vector2 containerSize);
    public virtual void Resize()
    {
        Parent?.Resize();
    }
}
