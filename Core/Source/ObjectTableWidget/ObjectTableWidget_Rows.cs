using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;
using Verse;

namespace Stats;

internal sealed partial class ObjectTableWidget<TObject>
{
    private void PinRow(int index)
    {
        List<TableRow<TObject>> rows = _filteredRows;
        TableRow<TObject> row = rows[index];
        int firstUnpinnedRowIndex = _pinnedRowsCount;
        TableRow<TObject> firstUnpinnedRow = rows[firstUnpinnedRowIndex];
        rows[firstUnpinnedRowIndex] = row;
        rows[index] = firstUnpinnedRow;
        _pinnedRowsCount++;
    }

    private void UnpinRow(int index)
    {
        List<TableRow<TObject>> rows = _filteredRows;
        TableRow<TObject> row = rows[index];
        int lastPinnedRowIndex = _pinnedRowsCount - 1;
        TableRow<TObject> lastPinnedRow = rows[lastPinnedRowIndex];
        rows[lastPinnedRowIndex] = row;
        rows[index] = lastPinnedRow;
        _pinnedRowsCount--;
    }

    //private readonly struct Row(int index, TObject @object)
    //{
    //    public readonly int Index = index;
    //    public readonly TObject Object = @object;
    //}

    //private sealed class Row
    //{
    //    public float Height;
    //    public readonly Cell[] Cells;
    //    public readonly int CellsCount;
    //    //public readonly TObject Object;

    //    private bool _isHovered = false;
    //    private readonly ObjectTableWidget<TObject> _parent;

    //    public Row(List<Column> columns, TObject @object, ObjectTableWidget<TObject> parent) : base()
    //    {
    //        // TODO: We can find all columns that are compatible with the table and create cells array
    //        // the size of count of these columns so we won't have to resize it when we will add columns later.
    //        int columnsCount = columns.Count;
    //        Cell[] cells = new Cell[columnsCount];
    //        for (int i = 0; i < columnsCount; i++)
    //        {
    //            Column column = columns[i];
    //            Cell cell = column.MakeCell(@object);
    //            cells[column.CellIndex] = cell;
    //        }

    //        Cells = cells;
    //        CellsCount = columnsCount;
    //        _parent = parent;
    //        //Object = @object;
    //    }

    //    public void Draw(Rect rect, ReadOnlyListSegment<Column> columns, float offsetX, int index)
    //    {
    //        bool mouseIsOverRect = Mouse.IsOver(rect);
    //        DrawBackground(rect, mouseIsOverRect, index);

    //        rect.x = -offsetX;

    //        Cell[] cells = Cells;
    //        int columnsCount = columns.Length;
    //        for (int i = 0; i < columnsCount; i++)
    //        {
    //            Column column = columns[i];
    //            rect.width = column.Width;
    //            Cell cell = cells[column.CellIndex];
    //            try
    //            {
    //                cell.Draw(rect);
    //            }
    //            catch
    //            {
    //                // TODO: ?
    //            }

    //            rect.x = rect.xMax;
    //        }

    //        // This must go after cells to not interfere with their GUI events.
    //        if (mouseIsOverRect && Event.current is { control: true, type: EventType.MouseDown })
    //        {
    //            HandlePinning(index);
    //        }
    //    }

    //    [MethodImpl(MethodImplOptions.AggressiveInlining)]
    //    private void DrawBackground(Rect rect, bool mouseIsOverRect, int index)
    //    {
    //        if (mouseIsOverRect)
    //        {
    //            _isHovered = true;
    //        }

    //        if (Event.current.type == EventType.Repaint)
    //        {
    //            // _isHovered may be true even if mouse is not over rect.
    //            if (_isHovered)
    //            {
    //                Verse.Widgets.DrawHighlight(rect);
    //            }
    //            else if (index % 2 == 0)
    //            {
    //                Verse.Widgets.DrawLightHighlight(rect);
    //            }
    //        }

    //        _isHovered = mouseIsOverRect;
    //    }

    //    [MethodImpl(MethodImplOptions.NoInlining)]
    //    private void HandlePinning(int index)
    //    {
    //        if (index > _parent._pinnedRowsCount - 1)
    //        {
    //            _parent._guiAction = () => _parent.PinRow(index);
    //        }
    //        else
    //        {
    //            _parent._guiAction = () => _parent.UnpinRow(index);
    //        }
    //    }
    //}
}
