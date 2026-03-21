using System.Collections.Generic;
using UnityEngine;
using static Stats.GUIStyles.Table;

namespace Stats;

internal sealed partial class ObjectTable<TObject>
{
    private void RecalcLayout()
    {
        List<Column> columns = _columns;
        int columnsCount = _columns.Count;
        int pinnedColumnsCount = _leftColumnsCount;
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

        float pinnedRowsHeight = _topRowsCount * RowHeight;
        float unpinnedRowsHeight = BottomRowsCount * RowHeight;
        float contentHeight = RowHeight + pinnedRowsHeight + unpinnedRowsHeight;

        _topRowsHeight = pinnedRowsHeight;
        _bottomRowsHeight = unpinnedRowsHeight;
        _leftColumnsWidth = pinnedColumnsWidth;
        _contentSize = new Vector2(contentWidth, contentHeight);
    }
}
