using System.Collections.Generic;
using Stats.ColumnWorkers.Cells;
using Stats.Filters;
using Stats.TableWorkers;
using UnityEngine;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class OwnedByColonyColumnWorker(ColumnDef columnDef) : ColumnWorker<DefBasedObject, OwnedByColonyColumnWorker.LiveBooleanCell>
{
    public override ColumnDef Def => columnDef;
    public override ColumnType Type => ColumnType.Boolean;

    protected override LiveBooleanCell MakeCell(DefBasedObject @object)
    {
        return @object.Def is Verse.ThingDef thingDef
            ? new LiveBooleanCell(GetValue(thingDef), thingDef)
            : default;
    }

    protected override LiveBooleanCell RefreshCell(LiveBooleanCell cell, out bool wasStale)
    {
        bool value = InventoryStateTracker.IsOwnedByPlayer(cell.ThingDef);
        wasStale = value != cell.Value;
        return wasStale ? new LiveBooleanCell(value, cell.ThingDef) : cell;
    }

    public override float GetWidth(List<int> rows)
    {
        return Verse.Text.LineHeight;
    }

    public override ICollection<CellField> GetCellFields(TableWorker tableWorker)
    {
        Filter valueFieldFilter = new BooleanFilter(row => this[row].Value);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        return [new CellField(Def.TitleWidget, valueFieldFilter, Compare)];
    }

    private static bool GetValue(Verse.ThingDef thingDef)
    {
        return InventoryStateTracker.IsOwnedByPlayer(thingDef);
    }

    public readonly struct LiveBooleanCell : IBooleanCell
    {
        public float Width => 0f;
        public bool IsRefreshable => true;
        public bool Value { get; }
        public Verse.ThingDef ThingDef { get; }

        public LiveBooleanCell(bool value, Verse.ThingDef? thingDef = null)
        {
            Value = value;
            ThingDef = thingDef!;
        }

        public void Draw(Rect rect)
        {
            BooleanCell.Draw(rect, Value);
        }
    }
}
