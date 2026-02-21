using UnityEngine;

namespace Stats.ObjectTable.Cells;

public abstract class Cell
{
    public Vector2 Size;
    public abstract void Draw(Rect rect);
    public abstract void Refresh();
}
