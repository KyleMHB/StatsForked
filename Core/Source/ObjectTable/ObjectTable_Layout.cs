using System.Collections.Generic;
using UnityEngine;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
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
            if (column.IsWidthSetManually == false)
            {
                column.RecalcWidth(_rows);
            }

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
        float unpinnedRowsHeight = UnpinnedRowsCount * _rowHeight;
        float contentHeight = _rowHeight + pinnedRowsHeight + unpinnedRowsHeight;

        _pinnedRowsHeight = pinnedRowsHeight;
        _unpinnedRowsHeight = unpinnedRowsHeight;
        _pinnedColumnsWidth = pinnedColumnsWidth;
        _contentSize = new Vector2(contentWidth, contentHeight);
    }
}
