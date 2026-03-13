using Stats.Utils;
using Stats.Utils.Extensions;
using UnityEngine;

namespace Stats.ColumnWorkers.Cells;

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
        if (value)
        {
            rect
                .ContractedByObjectTableCellPadding()
                .DrawTextureFitted(_textureTrue);
        }
    }
}
