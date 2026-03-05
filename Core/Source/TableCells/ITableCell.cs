using UnityEngine;

namespace Stats.TableCells;

public interface ITableCell
{
    public float Width { get; }
    public bool IsRefreshable { get; }

    public void Draw(Rect rect);
}
