using System.Collections.Generic;
using UnityEngine;

namespace Stats;

internal sealed partial class ObjectTableWidget<TObject>
{
    private void RecalcLayout()
    {
        List<Column> columns = _columns;
        int columnsCount = _columns.Count;
        int pinnedColumnsCount = _pinnedColumnsCount;
        float pinnedColumnsWidth = 0f;
        float unpinnedColumnsWidth = 0f;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            column.RecalcWidth(_rows);

            if (i < pinnedColumnsCount)
            {
                pinnedColumnsWidth += column.Width;
            }
            else
            {
                unpinnedColumnsWidth += column.Width;
            }
        }
        float contentWidth = pinnedColumnsWidth + unpinnedColumnsWidth;

        float pinnedRowsHeight = _pinnedRowsCount * _rowHeight;
        float unpinnedRowsHeight = (_rows.Count - _pinnedColumnsCount) * _rowHeight;
        float contentHeight = _rowHeight + pinnedRowsHeight + unpinnedRowsHeight;

        _pinnedRowsHeight = pinnedRowsHeight;
        _unpinnedRowsHeight = unpinnedRowsHeight;
        _pinnedColumnsWidth = pinnedColumnsWidth;
        _contentSize = new Vector2(contentWidth, contentHeight);
    }
}
