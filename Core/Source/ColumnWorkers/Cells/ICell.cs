using UnityEngine;

namespace Stats.ColumnWorkers.Cells;

public interface ICell
{
    public float Width { get; }
    public bool IsRefreshable { get; }

    public void Draw(Rect rect);
}
