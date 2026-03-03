using UnityEngine;

namespace Stats.TableCells;

public interface ITableCell
{
    public float Width { get; }

    public void Draw(Rect rect);
}
