using UnityEngine;

namespace Stats.ObjectTable.Cells;

public interface ICell
{
    public float Width { get; }

    public void Draw(Rect rect);
}
