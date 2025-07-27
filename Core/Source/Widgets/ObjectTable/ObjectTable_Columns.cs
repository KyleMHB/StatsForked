using UnityEngine;

namespace Stats.Widgets;

internal sealed partial class ObjectTable<TObject>
{
    private readonly Column[] Columns;
    private readonly Widget ColumnsTabWidget;
    private void ResetColumnsWidths()
    {
        foreach (var column in Columns)
        {
            column.Width = 0f;
        }
    }
    private void RecalcColumnsWidths()
    {
        FilteredBodyRows.RecalcColumnsWidths();
        PinnedRows.RecalcColumnsWidths();
        HeaderRows.RecalcColumnsWidths();
    }

    private sealed class Column
    {
        public bool IsPinned;
        public float Width;
        public readonly TextAnchor TextAnchor;
        public readonly ColumnWorker<TObject> Worker;
        // TODO: This is a quick and dirty way of hiding empty columns.
        // If there was no cells generated for a column, it will have width = 0. That's why, rows are initialized first.
        // The columns are still there, they are just ignored. Shouldn't cause any issues, but this is obviously inefficient.
        // Note to myself: When you'll get to refactoring this, don't forget to remove checks for width = 0.
        public bool IsVisible = true;
        private readonly ObjectTable<TObject> Parent;
        public Column(ColumnWorker<TObject> worker, bool isPinned, ObjectTable<TObject> parent)
        {
            Worker = worker;
            IsPinned = isPinned;
            TextAnchor = (TextAnchor)worker.CellStyle;
            Parent = parent;
        }
        public void Toggle()
        {
            IsVisible = !IsVisible;

            Parent.RecalcRowsHeight();
        }
    }
}
