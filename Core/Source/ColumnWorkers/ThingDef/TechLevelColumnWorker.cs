using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.FilterWidgets;
using Stats.TableCells;
using Stats.TableWorkers;
using UnityEngine;
using Verse;

namespace Stats.ColumnWorkers.ThingDef;

public sealed class TechLevelColumnWorker(ColumnDef columnDef) : ColumnWorker<DefBasedObject, TechLevelColumnWorker.TechLevelCell>
{
    public override ColumnDef Def => columnDef;

    protected override TechLevelCell MakeCell(DefBasedObject @object)
    {
        if (@object.Def is Verse.ThingDef thingDef)
        {
            return new TechLevelCell(thingDef.techLevel);
        }

        return default;
    }

    public override TableCellDescriptor GetCellDescriptor(TableWorker tableWorker)
    {
        IEnumerable<NTMFilterOption<TechLevel>> valueFieldFilterOptions = ((IRefRecordsProvider<Verse.ThingDef>)tableWorker).Records
            .Select(thingDef => thingDef.techLevel)
            .Distinct()
            .OrderBy(techLevel => techLevel)
            .Select<TechLevel, NTMFilterOption<TechLevel>>(
                techLevel => new(techLevel, techLevel.ToStringHuman().CapitalizeFirst())
            );
        FilterWidget valueFieldFilter = new OTMFilter<TechLevel>((int row) => this[row].Value, valueFieldFilterOptions);
        int Compare(int row1, int row2) => this[row1].Value.CompareTo(this[row2].Value);
        TableCellFieldDescriptor valueField = new(Def.Title, valueFieldFilter, Compare);

        return new TableCellDescriptor(TableCellStyleType.String, [valueField]);
    }

    public readonly struct TechLevelCell : ITableCell
    {
        public float Width { get; }
        public bool IsRefreshable => false;
        public readonly TechLevel Value;

        private readonly string? _text;

        public TechLevelCell(TechLevel techLevel)
        {
            Value = techLevel;
            if (techLevel != TechLevel.Undefined)
            {
                _text = techLevel.ToStringHuman().CapitalizeFirst();
                Width = Text.CalcSize(_text).x;
            }
        }

        public void Draw(Rect rect)
        {
            if (Value != TechLevel.Undefined)
            {
                rect = rect.ContractedByObjectTableCellPadding();
                Widgets.Draw.Label(rect, _text, TableCellStyle.String);
            }
        }
    }
}
