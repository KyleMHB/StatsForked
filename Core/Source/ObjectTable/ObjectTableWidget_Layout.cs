using System;
using System.Collections.Generic;
using Stats.ObjectTable.Cells;
using UnityEngine;

namespace Stats.ObjectTable;

internal sealed partial class ObjectTableWidget<TObject>
{
    private void RecalcLayout()
    {
        List<Column> columns = _columns;
        int columnsCount = _columns.Count;
        float headerRowHeight = 0f;
        Span<float> columnWidths = stackalloc float[columnsCount];
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            int cellIndex = column.CellIndex;
            Vector2 headerCellSize = column.HeaderCellSize;
            columnWidths[cellIndex] = headerCellSize.x;

            if (headerRowHeight < headerCellSize.y)
            {
                headerRowHeight = headerCellSize.y;
            }
        }

        List<Row<TObject>> rows = _rows;
        int rowsCount = rows.Count;
        int pinnedRowsCount = _pinnedRowsCount;
        float pinnedRowsHeight = 0f;
        float unpinnedRowsHeight = 0f;
        for (int i = 0; i < rowsCount; i++)
        {
            Row<TObject> row = rows[i];
            float rowHeight = 0f;
            Cell[] cells = row.Cells;
            int cellsCount = cells.Length;
            for (int j = 0; j < cellsCount; j++)
            {
                Cell cell = cells[j];
                Vector2 cellSize = cell.Size;

                if (rowHeight < cellSize.y)
                {
                    rowHeight = cellSize.y;
                }

                if (columnWidths[j] < cellSize.x)
                {
                    columnWidths[j] = cellSize.x;
                }
            }
            row.Height = rowHeight;

            if (i < pinnedRowsCount)
            {
                pinnedRowsHeight += rowHeight;
            }
            else
            {
                unpinnedRowsHeight += rowHeight;
            }
        }
        float contentHeight = headerRowHeight + pinnedRowsHeight + unpinnedRowsHeight;

        int pinnedColumnsCount = _pinnedColumnsCount;
        float pinnedColumnsWidth = 0f;
        float unpinnedColumnsWidth = 0f;
        for (int i = 0; i < columnsCount; i++)
        {
            Column column = columns[i];
            int cellIndex = column.CellIndex;
            column.Width = columnWidths[cellIndex];

            if (i < pinnedColumnsCount)
            {
                pinnedColumnsWidth += columnWidths[cellIndex];
            }
            else
            {
                unpinnedColumnsWidth += columnWidths[cellIndex];
            }
        }
        float contentWidth = pinnedColumnsWidth + unpinnedColumnsWidth;

        _headerRowHeight = headerRowHeight;
        _pinnedRowsHeight = pinnedRowsHeight;
        _unpinnedRowsHeight = unpinnedRowsHeight;
        _pinnedColumnsWidth = pinnedColumnsWidth;
        _unpinnedColumnsWidth = unpinnedColumnsWidth;
        _contentSize = new Vector2(contentWidth, contentHeight);
    }
}
