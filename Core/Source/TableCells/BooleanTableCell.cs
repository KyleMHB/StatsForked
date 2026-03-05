using UnityEngine;

namespace Stats.TableCells;

public interface IBooleanTableCell : ITableCell
{
    public bool Value { get; }
}

public readonly struct BooleanTableCell : IBooleanTableCell
{
    private static readonly Texture2D _textureTrue = Verse.Widgets.CheckboxOnTex;

    public float Width => 0f;
    public bool IsRefreshable => false;
    public bool Value { get; }

    public BooleanTableCell(bool value)
    {
        Value = value;
    }

    public void Draw(Rect rect) => Draw(rect, Value);

    public static void Draw(Rect rect, bool value)
    {
        if (value && Event.current.type == EventType.Repaint)
        {
            // TODO: Make it not take full available space.
            rect = rect.ContractedByObjectTableCellPadding();
            Verse.Widgets.DrawTextureFitted(rect, _textureTrue, 1f);
        }
    }
}
