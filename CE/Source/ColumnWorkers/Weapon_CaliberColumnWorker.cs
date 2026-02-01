using System;
using System.Collections.Generic;
using System.Linq;
using RimWorld;
using Stats.Objects.ThingDef;
using Stats.Widgets;
using Verse;

namespace Stats.Compat.CE;

public sealed class Weapon_CaliberColumnWorker : ColumnWorker<VirtualThing>
{
    public Weapon_CaliberColumnWorker(ColumnDef columnDef) : base(columnDef, CellStyleType.String, TODO)
    {
    }
    public override ObjectTableWidget.Cell GetCell(VirtualThing thing)
    {
        return new Cell(thing, this);
    }
    private string? GetCaliberName(StatRequest statRequest)
    {
        if (StatDefOf.Caliber.Worker.ShouldShowFor(statRequest))
        {
            return StatDefOf.Caliber.Worker.GetStatDrawEntryLabel(
                StatDefOf.Caliber,
                StatDefOf.Caliber.Worker.GetValue(statRequest),
                ToStringNumberSense.Absolute,
                statRequest
            );
        }

        return null;
    }
    private string? GetCaliberName(VirtualThing thing)
    {
        var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
        var statRequest = StatRequest.For(thingDef, null);

        return GetCaliberName(statRequest);
    }
    public override IEnumerable<ObjectProp> GetObjectProps(IEnumerable<VirtualThing> contextObjects)
    {
        var options = contextObjects
            .Select(GetCaliberName)
            .Distinct()
            .OrderBy(option => option)
            .Select<string?, NTMFilterOption<string?>>(
                option => option == null ? new() : new(option, option.ToString())
            );

        yield return new(Def.Title, new OTMFilter<Cell, string?>(cell => cell.Value, options, this));
    }

    private sealed class Cell : ObjectTableWidget.WidgetCell
    {
        protected override Widget? Widget { get; set; }
        public override event Action? OnChange;
        public string? Value { get; }
        public Cell(VirtualThing thing, Weapon_CaliberColumnWorker column)
        {
            var thingDef = thing.Def.building?.turretGunDef ?? thing.Def;
            var statRequest = StatRequest.For(thingDef, null);
            var caliberName = column.GetCaliberName(statRequest);

            if (caliberName?.Length > 0)
            {
                Widget widget = new Label(caliberName).PaddingAbs(ObjectTableWidget.CellPadHor, ObjectTableWidget.CellPadVer);
                var tooltip = StatDefOf.Caliber.Worker.GetExplanationFull(
                    statRequest,
                    ToStringNumberSense.Absolute,
                    StatDefOf.Caliber.Worker.GetValue(statRequest)
                );

                if (tooltip?.Length > 0)
                {
                    widget = widget.Tooltip(tooltip);
                }

                Widget = widget;
                Value = caliberName;
            }
        }
        public override int CompareTo(ObjectTableWidget.Cell cell)
        {
            return Comparer<string?>.Default.Compare(Value, ((Cell)cell).Value);
        }
    }
}
